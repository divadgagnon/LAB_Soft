   M S B A M L   `   `   `  ����
  LAB_V2    LAB.Views.ConnectionSetup   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0ignorehttp://www.galasoft.ch/ignore     5   	     � WindowStartupLocation$  CenterScreen=�5   	    � Title$ Connection Setup��5	   	   $	��200��5
   L   $	��200��6Y   #��� ��5   	    � DataContext 5
   	   �� *	PortSetup+   Locator �� Source# [   � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $+ %/LAB_V2;component/Skins/MainSkin.xaml?�6%   %    5      .��5      �� $��0,0,0,0q�6   .+�5   
   �� -   5         ��Name$ label��5      $��Select a Serial Port��6   $��center=�6>   $��10q�6[   $	��Top=�6g   $
��Auto��6     Principal_Forground#���  6�   �� 5   
   -   5         ��Name$ comboBox��5      $��Center=�6�   $��10q�6�   $��Center=�6�   $	��100��6�   $��26��6�   $��16(�6�   ��6%   �� 	 �� Path$	 comPortList�
 �� SelectedItem
 6N   �� $	 SelectedcomPort� �� Mode$ TwoWay=�6�   �� 5   
   $	��100��6e   $��25��6q   $��30q�6}     Button_LABSTYLE#���  6�   ��6   �� $	 ConnectButtonContent���6?   �� $	 ConnectProbe�6�   5      5      