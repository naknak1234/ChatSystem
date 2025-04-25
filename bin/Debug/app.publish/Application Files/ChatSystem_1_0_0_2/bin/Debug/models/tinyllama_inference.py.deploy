import sys
import json
import os
import logging
import torch
import psutil
from transformers import AutoModelForCausalLM, AutoTokenizer, BitsAndBytesConfig

logging.basicConfig(
    filename='inference.log',
    level=logging.DEBUG,
    format='%(asctime)s - %(levelname)s - %(message)s'
)

def check_memory_and_set_device(max_allowed_ram=15):
    """Check available memory (GPU and CPU) and set the device."""
    if torch.cuda.is_available():
        total_gpu_memory = torch.cuda.get_device_properties(0).total_memory / (1024 ** 3) 
        allocated_gpu_memory = torch.cuda.memory_allocated(0) / (1024 ** 3)  
        free_gpu_memory = total_gpu_memory - allocated_gpu_memory
        logging.debug(f"GPU Memory - Total: {total_gpu_memory:.2f}GB, Free: {free_gpu_memory:.2f}GB")
        if free_gpu_memory > 5:
            return "cuda"

    ram = psutil.virtual_memory()
    total_ram = ram.total / (1024 ** 3) 
    available_ram = ram.available / (1024 ** 3) 
    logging.debug(f"RAM - Total: {total_ram:.2f}GB, Available: {available_ram:.2f}GB")

    if available_ram < max_allowed_ram:
        logging.warning(f"Available RAM ({available_ram:.2f}GB) is less than the maximum allowed ({max_allowed_ram}GB).")
    else:
        logging.debug(f"Using CPU with sufficient RAM: {available_ram:.2f}GB available")

    return "cpu"

def load_model_and_tokenizer(models_dir, model_name):
    """Load the model and tokenizer with dynamic configuration."""
    logging.debug(f"Loading model from {models_dir}/{model_name}")
    model_path = os.path.join(models_dir, model_name)
    if not os.path.exists(model_path):
        logging.error(f"Model directory {model_path} does not exist")
        raise ValueError(f"Model directory {model_path} does not exist.")

    device = check_memory_and_set_device(max_allowed_ram=15)
    logging.debug(f"Selected device: {device}")

    try:
        quantization_config = BitsAndBytesConfig(load_in_4bit=True)
        logging.debug("Loading model with 4-bit quantization")
        model = AutoModelForCausalLM.from_pretrained(
            model_path,
            quantization_config=quantization_config,
            device_map=device,
            torch_dtype="auto"
        )
    except ImportError:
        logging.warning("BitsAndBytes not installed, loading model without quantization")
        model = AutoModelForCausalLM.from_pretrained(
            model_path,
            device_map=device,
            torch_dtype="auto"
        )

    logging.debug("Loading tokenizer")
    tokenizer = AutoTokenizer.from_pretrained(model_path)
    return model, tokenizer, device
#naknak1234
def generate_response(input_text, model, tokenizer, device):
    """Generate a natural response using token-based extraction."""
    logging.debug(f"Generating response for input: {input_text}")
    use_chat_template = hasattr(tokenizer, 'apply_chat_template')
    if use_chat_template:
        messages = [
            {"role": "system", "content": "You are a helpful assistant who provides concise, natural responses."},
            {"role": "user", "content": input_text}
        ]
        formatted_input = tokenizer.apply_chat_template(
            messages,
            tokenize=False,
            add_generation_prompt=True
        )
        logging.debug("Using apply_chat_template for prompt formatting")
    else:
        formatted_input = f"{input_text}"
        logging.debug("Using simple prompt format")

    logging.debug(f"Formatted input: {formatted_input}")
    model_inputs = tokenizer([formatted_input], return_tensors="pt").to(device)
    input_ids = model_inputs.input_ids
    try:
        generated_ids = model.generate(
            input_ids,
            max_new_tokens=512,
            do_sample=True,
            temperature=0.7,
            top_p=0.9,
            pad_token_id=tokenizer.eos_token_id
        )
    except RuntimeError as e:
        logging.error(f"Generation failed: {str(e)}")
        raise RuntimeError(f"Generation failed: {str(e)}")
    generated_ids = generated_ids[:, input_ids.shape[1]:]
    response = tokenizer.batch_decode(generated_ids, skip_special_tokens=True)[0]
    logging.debug(f"Decoded response: {response}")
    return response.strip()

if __name__ == "__main__":
    if len(sys.argv) < 4:
        logging.error("Invalid arguments. Usage: python tinyllama_inference.py \"your input\" \"model_name\" \"models_dir\"")
        print(json.dumps({"error": "Usage: python tinyllama_inference.py \"your input\" \"model_name\" \"models_dir\""}))
        sys.exit(1)
    input_text = sys.argv[1]
    model_name = sys.argv[2]
    models_dir = sys.argv[3]
    try:
        model, tokenizer, device = load_model_and_tokenizer(models_dir, model_name)
        response = generate_response(input_text, model, tokenizer, device)
        logging.info(f"Response generated: {response}")
        print(json.dumps({"response": response}))
    except Exception as e:
        logging.error(f"Error during inference: {str(e)}")
        print(json.dumps({"error": str(e)}))
        sys.exit(1)