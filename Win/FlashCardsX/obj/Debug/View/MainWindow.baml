   M S B A M L   `   `   `  ����  FlashCardsX!    FlashCardsX.View.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5        � Title$  FlashCardsX��5      $	��375��6   $	��700��6&    � 
ResizeMode$ NoResize=�62    J� InputBindings 5      �� 5   
   
 �� Key$ N��6    �� 	Modifiers$
 Ctrlf�6    �� Command 6.   ��  �� Path$ NewDeckCommand��� 5   
   $ V��6   $
 Ctrlf�6    6.   �� $ ViewDeckCommand��� 5   
   $ D��6   $
 Ctrlf�6    6.   �� $ DeleteDeckCommand��� 5	   
   $ T��6   $
 Ctrlf�6    6.   �� $ StartTestCommand��� 5
   
   $ E��6   $
 Ctrlf�6    6.   �� $ ExportPdfCommand��� 5   
   $ S��6   $
 Ctrlf�6    6.   �� $ SettingsCommand�.��5      �  � RowDefinitions 5   
   �� 5      �� 5      $��20 �6   .+�5   
   �  � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $	��166 �6#   .+�5      ]� $��0��6   .+�5      �� .��5      &   System.Windows.Controls.DataGrid  -   5        $��DeckListDataGrid��5      	   AutoGenerateColumns	 .  
   
IsReadOnly
 .    SelectionMode$ Single=�5        �� SelectedIndex 67   �� $ SelectedDeck� �� Mode$ TwoWay=���5      �� $ DeckList�   Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 	Deck Name��61   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6D   �� *Name+  5      $ Topic A��61    6B   �� *TopicA+  5      $ Topic B��61    6B   �� *TopicB+  5       $
 Size��61    6?   �� *Size+  5!         Width5  /System.Windows.Controls.DataGridLengthConverter$ * 61   $ Last Modified��6;    6R   �� *	LastMod+5#      5$      5%      �� 5&      $��1��6   .+�5'      �� -   5        $��NewDeckButton��5'      ��6.   �� $ NewDeckCommand�.��5(      �� B� 6!   .�6+   N6.   	ew Deck6A   5)      �� 5*      -   5        $��ViewDeckButton��5*      ��6/   �� $ ViewDeckCommand�.��5+      �� B� 6!   .�6+   V6.   
iew Deck6B   5,      �� 5-      -   5        $��DeleteButton��5-      ��6-   �� $ DeleteDeckCommand�.��5.      �� B� 6!   .�6+   D6.   
elete Deck6D   5/      �� 50      -   5        $��StartButton��50      ��6,   �� $ StartTestCommand�.��51      �� Start B� 6'   .�61   T64   esting6F   52      �� 53      -   5        $��	PdfButton��53      ��6*   �� $ ExportPdfCommand�.��54      �� B� 6!   .�6+   E6.   xport to PDF6F   55      �� 56      $��4q�6   $
��Bold"�6'   Difficulty:58      �� 59      -   5        $��ComparatorComboBox��59       65   �� $ 
Comparator�.@�5:      �� .��6#   
At Least6-   �� 5;      .��6#   	At Most6,   5<      �� 5=      -   5        $��DifficultyComboBox��5=       65   �� $ 
Difficulty�.@�5>      �� .��6#   Easy6)   �� 5?      .��6#   Medium6+   �� 5@      .��6#   Hard6)   5A      �� 5B      -	   5        $��FlipCheckBox��5B      $��5q�6/    d� 	IsChecked 6:   �� $ Flipped�$ OneWayToSource=�.��6r   	Flipped5D      �� 5E      -
   5        $��ShuffleCheckBox��5E      $��5q�62    6=   �� $ Shuffled�$ OneWayToSource=�.��6v   
Shuffled5G      �� 5H      -   5        $��OptionsButton��5H      ��6.   �� $ OptionsCommand�.��5I      �� B� 6!   .�6+   S6.   	ettings6A   5J      �� 5K      $��4q�6   Status6/   �� 5L      -   5        $��SkyDriveLabel��5L   R   $��4q�6   ��6'   �� $ SkyDriveStatus�
OneDrive6q   �� 5M      -   5        $��DropboxLabel��5M   Q   $��4q�6   ��6'   �� $ DropboxStatus�	Dropbox6n   5N      5O      �� 5P   
   $��1��6   
���   �.@�5Q      �� .��5R      �� -   5        $��ItemsLoadedLabel��5R      ��64   �� $ 	CountText�5S      �� 5T      .��5U      �� -   5        $��	HelpLabel��5U      ��5V      �� $ HelpText� �� NotifyOnTargetUpdated . $ TwoWay=� � Triggers 5W      :� 5X       :� RoutedEvent$ Binding.TargetUpdated��6'   .[�5Y      �� .��5Z   "   �� .��5[   &   W�  �� TargetProperty$ Opacity�66    W� From$ 1X�6Z   	 W� To$ 0X�6c    g� Duration$ 0:0:3D�6j   5\   #   5]      5^      5`      5a      5b      5c      5d      