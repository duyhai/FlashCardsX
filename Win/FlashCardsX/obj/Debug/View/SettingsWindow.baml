   M S B A M L   `   `   `  ����  FlashCardsX%    FlashCardsX.View.SettingsWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Settings��5   	   $	��300��6    � 
ResizeMode$ NoResize=�6&    � Topmost .  � SizeToContent$ Height=�6K    � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5         ExitCommand��%   &	        �� 5   
   6   5       J� CommandBindings 5	      )  #System.Windows.Input.CommandBinding  5
   
   -   5           Command# [  5
      6@    J� InputBindings 5      �� 5   
   
	 �� Key$	 E��6   
 �� 	Modifiers$

 Ctrlf�6    �� Command# [  6.   �� 5   
   $	 S��6   $

 Ctrlf�6    6.   ��  �� Path$ SaveCommand�.��5      � .+�5   
   �� -   5        $��SettingsStackPanel��5      $��10q�6/   $��Center=�6;   .+�5      �� $��4q�6   
Local Path6/   �� 5      -   5        $��LocalPathTextBox��5      ��6.   �� *	LocalPath+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� 5      $��4q�6   PDF Page Size62   �� 5       �� Orientation$ 
Horizontal=�6   .+�5      �� $��4q�6   W:6,   �� 5      -   5        $��Center=�5      $	��100��6<   ��6H   �� $ PdfWidth�$ PropertyChanged=�6�   �� 5      $��4q�6   x6*   �� 5      $��4q�6   H:6+   �� 5      -   5        $��Center=�5      $	��100��6<   ��6H   �� $ 	PdfHeight�$ PropertyChanged=�6�   5      �� 5      -   5        $��SkyDriveCheckBox��5      $��
0, 5, 0, 0q�5       d� 	IsChecked 6+   �� *
SkyDrive+$ TwoWay=�.��5   /   
OneDrive5      �� 5      $��4q�6   User6)   �� 5       -   5        $��SkyDriveUserTextBox��5    (   {�.  ��6C   �� $ SkyDriveUser��� 5!      -   5        $��SkyDriveSignInButton��5!      $��
0, 5, 0, 0q�61   {�5"      �� *	IsChecked+ �� ElementName$ SkyDriveCheckBox����5#      �� $ SkyDriveSignInCommand���5$      �� $ SkyDriveWebBrowser��.��5%      �� ��6   �� $ SkyDriveSignInText�5&      (  "System.Windows.Controls.WebBrowser  5'      -	   5        $��SkyDriveWebBrowser��5'   &   $	��250��6   $y�	Collapsed=�6@   �� 5(      -
   5        $��DropboxCheckBox��5(      $��
0, 5, 0, 0q�6.    6B   �� *	Dropbox+$ TwoWay=�.��6m   	Dropbox5)      �� 5*      $��4q�6   User6)   �� 5+      -   5        $��DropboxUserTextBox��5+   (   {�.  ��6B   �� $ DropboxUser��� 5-      -   5        $��DropboxSignInButton��5-      $��
0, 5, 0, 0q�60   {�5.      �� *	IsChecked+$ DropboxCheckBox����5/      �� $ DropboxSignInCommand���50      �� $ DropboxWebBrowser��.��51      �� ��6   �� $ DropboxSignInText�52        53      -   5        $��DropboxWebBrowser��53   &   $	��250��6   $y�	Collapsed=�6?   �� 54      -   5        $��StatusTextBlock��54      ��6/   �� *Status+ �� NotifyOnTargetUpdated . $ TwoWay=� � Triggers 55      :� 56       :� RoutedEvent$ Binding.TargetUpdated��6#   .[�57      �� .��58      �� .��59   "   W�  �� TargetProperty$ Opacity�62    W� From$ 1X�6V   	 W� To$ 0X�6_    g� Duration$ 0:0:3D�6f   5:      5;      5<      5>      �� 5?      -   5        $��
SaveButton��5?   )   $��
0, 5, 0, 0q�6   ��6;   �� $ SaveCommand�.��5@      �� B� 5A      .�6    S6#   ave5B      5C      �� 5D      -   5        $��
ExitButton��5D      #��[  6'   .��5E      �� B� 5F      .�6    E6#   xit5G      6   5H      5I      5J      