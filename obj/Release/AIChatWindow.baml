   M S B A M L   `   `   `  ����   
ChatSystem    ChatSystem.AIChatWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  AI Chat��$	��600��$	��900�����#E8ECEF � 	Resources    WindowGradient �� 
StartPoint �� EndPoint � Color � Offset  ModernButtonStyle �� 
TargetType �� Property �� Value3  -System.Windows.Media.Effects.DropShadowEffect	   Color
   	Direction   ShadowDepth   Opacity   
BlurRadius �� 
TargetType ��Name �� CornerRadius ��Name �� Orientation
 � Tag �� CenterX �� CenterY � Triggers :� RoutedEvent �� 
TargetName �� TargetProperty W� From	 W� To g� Duration g� AutoReverse g� RepeatBehavior �� Triggers  P� Property! P� Value" �� 
TargetName  RoundedTextBoxStyle# ��Name  ChatMessageStyle$ �� Triggers  ChatSessionItemStyle��%�	  &	        &	 R     &	 e    &	 �    &	 Y    �� $	 0,0&�$	 1,1&�.��� $ #A3BFFA��$ 0Q�� $ #E8ECEF��$ 1Q��� " ��.���  � �� �� $	 0,0&�$	 1,1&�.��� $ #6B7280��$ 0Q�� $ #4B5563��$ 1Q���  � ��
 �������  � ��$ 14(���  � ��$ Segoe UI)���  � ��$
 12,6q���  � ��$ 0,10,0,0q���  � ��$ 0q���  �@J�Effect   $	 #000000��$	
 315Q�$ 2Q�$	 0.3Q�$ 5Q���  � �� �� " ��.R�� .+��� $ ButtonBorder��#��z��$ 15��._��� $ NormalContent��$ 
Horizontal=�$��Center=�$��Center=�.+��� #��z $��Segoe MDL2 Assets)�$��16(�$��0,0,8,0q��� $��Center=��� $ 
LoadingBar��
�������$��4��$��20��$ 2��$��Center=�$��Center=�$y�	Collapsed=�z��� BarScale��$ 10Q�$ 2Q� :� $ FrameworkElement.Loaded��.[��� .���� .��W� $ BarScale��$ ScaleX�$ 1X�$ 2X�$ 0:0:0.6D� . $ Forever�� P�   �@J�IsMouseOver! . . ���  � �� �� $	 0,0&�$	 1,1&�.��� $ #9CA3AF��$ 0Q�� $ #6B7280��$ 1Q�P�   � ��! . . ���  � �� �� $	 0,0&�$	 1,1&�.��� $ #4B5563��$ 0Q�� $ #374151��$ 1Q�P�   � {�! .  . ���  � �� �#D1D5DB�� $" ButtonBorder�� � �� �#B0BEC5�� $" NormalContent�� � y�$ 	Collapsed=��� $" 
LoadingBar�� � y�$ Visible=��� " ��.���  � �� �#F9FAFB��  � �� �#D1D5DB��  � ��$ 1q���  � ��$ 12q���  � ��$ 0,0,0,0q���  � ��$ 14(���  � ��$ Segoe UI)���  �@J�Effect   $	 #000000��$	
 315Q�$ 1Q�$	 0.2Q�$ 3Q���  � �� �� " ��.R��� $ 15��#��z��#��z��#��z��._��� $# PART_ContentHost��$��Center=��� " ��.���  � ��$
 Wrap=���  � ��$ 10q���  �@��Padding$ 12q���  � ��$ 14(���  � ��$ Segoe UI)���  � ��$	 500����  � �� �#F1F8E9��  � ��$
 Left=���  �@J�Effect   $	 #000000��$	
 315Q�$ 1Q�$	 0.2Q�$ 3Q�$ P�   � ��$! You:.*��. ���  � �� �#E3F2FD��  � ��$ Right=��� " ��.���  � ��
 ���� ��  � ��$ 10q���  � ��$ 5q���  � �� �� " ��.R��� #��z��$ 10��._���  P�   �@��
IsSelected! . . ���  � �� �#E8ECEFP�   �@J�IsMouseOver! . . ���  � �� �#D1D5DB.��� $��20q�% � RowDefinitions% �� $��* ��� $
��Auto �& � ColumnDefinitions& �� $	��250 ��� $��* �.+��� $��0��
�������$ 20��._��� $��20q�.+��� $��Chat Sessions��$��20(�$��Segoe UI)�$
��Bold"����#1F2937$��0,0,0,20q��� -   ' ��Name$' NewChatButton��$��New Chat��#��[ $	 ��$	��120��$��33���� -   $' DeleteChatButton��$��Delete Chat��#��[ $	 ��$	��120��$��33���� -   $' RenameChatButton��$��Rename Chat��#��[ $	 ��$	��120��$��33���� -   ( ��Name$( AIModelComboBox��$��	0,10,0,10q�$��14(�$��Segoe UI)�$��10q�.@��� $��	TinyLlama���� -   $' LogoutButton��$��Logout��#��[ $	 ��$	��120��$��33���� -   ) ��Name$) ChatSessionsList��$��0,10,0,0q�
�������$��0q����� .R��� * �� Padding$* 10q�$��14(�$��Segoe UI)����#4B5563���� *Title+���� " ��+ �� BasedOn#+ [ �� $��1��$��0��
�������$ 20��$��20,0,0,0q�._��� $��20q�$
��Auto=�$��Disabled=�.���� -   $) ChatDisplay������ .R��� #��[ ���� ���� " ��.���  � ��$ 0q���  � ��$ 0q���  � ��$ Stretch=��� $��1��$��1��
�������$ 20��$��	20,20,0,0q�._�]� $��20q�.+��� -   $' 
SendButton��$
��Send��#��[ $	 ��$	��100��$��Right=��� -	   , ��Name$, MessageInput��- �� AcceptsReturn- .  #��[ $��0,0,10,0q�