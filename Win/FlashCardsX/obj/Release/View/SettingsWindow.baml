   M S B A M L   `   `   `  ����   FlashCardsX%    FlashCardsX.View.SettingsWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  Settings��$	��300�� � 
ResizeMode$ NoResize=� � Topmost .  � SizeToContent$ Height=� � WindowStartupLocation$ CenterScreen=� � 	Resources    ExitCommand��%   &	        ��  J� CommandBindings )  #System.Windows.Input.CommandBinding  -      Command# [   J� InputBindings �� 
	 �� Key$	 E��
 �� 	Modifiers$

 Ctrlf� �� Command# [  �� $	 S��$

 Ctrlf� ��  �� Path$ SaveCommand�.��� .+��� -   $��SettingsStackPanel��$��10q�$��Center=�.+��� $��4q�
Local Path�� -   $��LocalPathTextBox������ *	LocalPath+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $��4q�PDF Page Size��  �� Orientation$ 
Horizontal=�.+��� $��4q�W:�� -   $��Center=�$	��100������ $ PdfWidth�$ PropertyChanged=��� $��4q�x�� $��4q�H:�� -   $��Center=�$	��100������ $ 	PdfHeight�$ PropertyChanged=��� -   $��SkyDriveCheckBox��$��
0, 5, 0, 0q� d� 	IsChecked �� *
SkyDrive+$ TwoWay=�.��
OneDrive�� $��4q�User�� -   $��SkyDriveUserTextBox��{�.  ���� $ SkyDriveUser��� -   $��SkyDriveSignInButton��$��
0, 5, 0, 0q�{��� *	IsChecked+ �� ElementName$ SkyDriveCheckBox������ $ SkyDriveSignInCommand����� $ SkyDriveWebBrowser��.���� ���� $ SkyDriveSignInText�(  "System.Windows.Controls.WebBrowser  -	   $��SkyDriveWebBrowser��$	��250��$y�	Collapsed=��� -
   $��DropboxCheckBox��$��
0, 5, 0, 0q� �� *	Dropbox+$ TwoWay=�.��	Dropbox�� $��4q�User�� -   $��DropboxUserTextBox��{�.  ���� $ DropboxUser��� -   $��DropboxSignInButton��$��
0, 5, 0, 0q�{��� *	IsChecked+$ DropboxCheckBox������ $ DropboxSignInCommand����� $ DropboxWebBrowser��.���� ���� $ DropboxSignInText�  -   $��DropboxWebBrowser��$	��250��$y�	Collapsed=��� -   $��StatusTextBlock������ *Status+ �� NotifyOnTargetUpdated . $ TwoWay=� � Triggers :�  :� RoutedEvent$ Binding.TargetUpdated��.[��� .���� .��W�  �� TargetProperty$ Opacity� W� From$ 1X�	 W� To$ 0X� g� Duration$ 0:0:3D��� -   $��
SaveButton��$��
0, 5, 0, 0q����� $ SaveCommand�.���� B� .�Save�� -   $��
ExitButton��#��[  .���� B� .�Exit