from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
import torch
from transformers import AutoModelForCausalLM, AutoTokenizer
import uvicorn

app = FastAPI()
model_path = "E:/OOAD/ChatSystem/bin/Debug/models/TinyLlama-1.1B-Chat-v1.0"
tokenizer = AutoTokenizer.from_pretrained(model_path)
model = AutoModelForCausalLM.from_pretrained(model_path, trust_remote_code=True, ignore_mismatched_sizes=True)
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
model.to(device)
model.eval()
class InferenceRequest(BaseModel):
    input: str
    max_tokens: int = 200
    temperature: float = 0.7
    top_p: float = 0.9
@app.post("/generate")
async def generate(request: InferenceRequest):
    try:
        formatted_input = f"<|user|>{request.input}<|assistant>"
        inputs = tokenizer(formatted_input, return_tensors="pt").to(device)
        input_ids = inputs["input_ids"]
        with torch.no_grad():
            outputs = model.generate(
                input_ids,
                max_new_tokens=request.max_tokens,
                temperature=request.temperature,
                top_p=request.top_p,
                do_sample=True,
                pad_token_id=tokenizer.eos_token_id
            )
        response = tokenizer.decode(outputs[0], skip_special_tokens=True)
        response = response.replace(formatted_input, "").strip()
        return {"response": response}
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating response: {str(e)}")
if __name__ == "__main__":
    uvicorn.run(app, host="127.0.0.1", port=8000)