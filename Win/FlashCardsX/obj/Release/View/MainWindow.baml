   M S B A M L   `   `   `  ����   FlashCardsX!    FlashCardsX.View.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml       � Title$  FlashCardsX��$	��375��$	��700�� � 
ResizeMode$ NoResize=� J� InputBindings �� 
 �� Key$ N�� �� 	Modifiers$
 Ctrlf� �� Command ��  �� Path$ NewDeckCommand��� $ V��$
 Ctrlf� �� $ ViewDeckCommand��� $ D��$
 Ctrlf� �� $ DeleteDeckCommand��� $ T��$
 Ctrlf� �� $ StartTestCommand��� $ E��$
 Ctrlf� �� $ ExportPdfCommand��� $ S��$
 Ctrlf� �� $ SettingsCommand�.���  � RowDefinitions �� �� $��20 �.+��  � ColumnDefinitions �� $��* ��� $	��166 �.+�]� $��0��.+��� .��&   System.Windows.Controls.DataGrid  -   $��DeckListDataGrid��	   AutoGenerateColumns	 .  
   
IsReadOnly
 .    SelectionMode$ Single=� �� SelectedIndex �� $ SelectedDeck� �� Mode$ TwoWay=����� $ DeckList�   Columns 0  *System.Windows.Controls.DataGridTextColumn  ,  &System.Windows.Controls.DataGridColumn   Header$ 	Deck Name��1  +System.Windows.Controls.DataGridBoundColumn   Binding �� *Name+  $ Topic A�� �� *TopicA+  $ Topic B�� �� *TopicB+  $
 Size�� �� *Size+     Width5  /System.Windows.Controls.DataGridLengthConverter$ * $ Last Modified�� �� *	LastMod+�� $��1��.+��� -   $��NewDeckButton������ $ NewDeckCommand�.���� B� .�N	ew Deck�� -   $��ViewDeckButton������ $ ViewDeckCommand�.���� B� .�V
iew Deck�� -   $��DeleteButton������ $ DeleteDeckCommand�.���� B� .�D
elete Deck�� -   $��StartButton������ $ StartTestCommand�.���� Start B� .�Testing�� -   $��	PdfButton������ $ ExportPdfCommand�.���� B� .�Export to PDF�� $��4q�$
��Bold"�Difficulty:�� -   $��ComparatorComboBox�� �� $ 
Comparator�.@��� .��
At Least�� .��	At Most�� -   $��DifficultyComboBox�� �� $ 
Difficulty�.@��� .��Easy�� .��Medium�� .��Hard�� -	   $��FlipCheckBox��$��5q� d� 	IsChecked �� $ Flipped�$ OneWayToSource=�.��	Flipped�� -
   $��ShuffleCheckBox��$��5q� �� $ Shuffled�$ OneWayToSource=�.��
Shuffled�� -   $��OptionsButton������ $ OptionsCommand�.���� B� .�S	ettings�� $��4q�Status�� -   $��SkyDriveLabel��$��4q����� $ SkyDriveStatus�
OneDrive�� -   $��DropboxLabel��$��4q����� $ DropboxStatus�	Dropbox�� $��1��
���   �.@��� .���� -   $��ItemsLoadedLabel������ $ 	CountText��� .���� -   $��	HelpLabel������ $ HelpText� �� NotifyOnTargetUpdated . $ TwoWay=� � Triggers :�  :� RoutedEvent$ Binding.TargetUpdated��.[��� .���� .��W�  �� TargetProperty$ Opacity� W� From$ 1X�	 W� To$ 0X� g� Duration$ 0:0:3D�