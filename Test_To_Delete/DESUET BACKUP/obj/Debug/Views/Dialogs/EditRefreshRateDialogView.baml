   M S B A M L   `   `   `  ����
  LAB_V2)    #LAB.Views.EditRefreshRateDialogView   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0ignorehttp://www.galasoft.ch/ignore     5   	     � Title$   Refresh Rate Configuration��5   	    � WindowStartupLocation$ CenterScreen=�5	   	      WindowBackground#���   5
   P     Principal_Forground#���  6�   $��169.009��6�   $��165.415��6�    � DataContext 6	   �� *HardwareSetup+  Locator �� Source# [  � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $+ %/LAB_V2;component/Skins/MainSkin.xaml?�6%   %    5      .��5      �� $��Stretch=�6   $��Stretch=�6/    +� FocusedElement 6K   ��  �� ElementName$ Input��.+�5   
   �� $��Select a refresh rate��6   $��Center=�60   #���  6M   �� 5   
   -   5        	 ��Name$	 Input��5      $��Center=�6K   $��Center=�6o   $	��100��6�   $��10q�6�   ��6!   �� 
 �� Path$
 SelectedRefreshRate�6�   �� 5   
   $��Miliseconds��6   $��Center=�6&   #���  6C   �� 5   
   $��Confirm��6   $	��100��6#   $��10q�6/   ��6;   �� $
 ConfirmPinSelectedCommand�5      5      