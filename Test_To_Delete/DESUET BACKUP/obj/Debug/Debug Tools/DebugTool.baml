   M S B A M L   `   `   `  ����A  =LAB_V2, Version=0.0.0.1, Culture=neutral, PublicKeyToken=null1clr-namespace:LAB.Debug_ToolsLAB.Debug_Tools  5   	       LAB.Debug_Tools.DebugTool   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   /localclr-namespace:LAB.Debug_Tools     5   	     � Title$  	DebugTool��5   	   $��238.775��6   $��249.49��6,    � Topmost .  � DataContext 5
   	   �� *Debug+   Locator �� Source# [  .��5      � $��Center=�6   .+�5   
   �� .+�5      ��  �� Orientation$ 
Horizontal=�6   .+�5      �� $��L��6   $��3,0,0,0q�6$   �� 5      $	��°C��6   $��10,0,0,0q�6%   �� 5      $��L��6   $��35,0,0,0q�6$   �� 5      $	��°C��6   $��10,0,0,0q�6%   �� 5      $��L��6   $��37,0,0,0q�6$   �� 5      $	��°C��6   $��10,0,0,0q�6%   5      �� 5      $ 
Horizontal=�6   $��Center=�62   $	��Top=�6O   .+�5      �� -   5         ��Name$ HLTVOL��5       �� Orientation$ Vertical=�6\   $
��Left=�6s   $	��Top=�6�   $	��155��6�    � Maximum$ 50Q�6�    � Value 6)   �� 	 �� Path$	 HLTVOL�
 �� Mode$
 OneWayToSource=�6�   �� 5      -   5        $ HLTTEMP��5      $ Vertical=�6^   $
��Left=�6u   $	��Top=�6�   $	��155��6�   $��10,0,0,0q�6�   $	 100Q�6�    6*   �� $	 HLTTEMP�$
 OneWayToSource=�6�   �� 5      -   5        $ MLTVOL��5      $ Vertical=�6\   $
��Left=�6s   $	��Top=�6�   $	��155��6�   $��30,0,0,0q�6�   $ 50Q�6�    6)   �� $	 MLTVOL�$
 OneWayToSource=�6�   �� 5      -   5        $ MLTTEMP��5      $ Vertical=�6^   $
��Left=�6u   $	��Top=�6�   $	��155��6�   $��10,0,0,0q�6�   $	 100Q�6�    6*   �� $	 MLTTEMP�$
 OneWayToSource=�6�   �� 5      -   5        $ BKVOL��5      $ Vertical=�6Z   $
��Left=�6q   $	��Top=�6�   $	��155��6�   $��30,0,0,0q�6�   $ 50Q�6�    6(   �� $	 BKVOL�$
 OneWayToSource=�6�   �� 5      -   5        $ BKTEMP��5      $ Vertical=�6\   $
��Left=�6s   $	��Top=�6�   $	��155��6�   $��10,0,0,0q�6�   $	 100Q�6�    6)   �� $	 BKTEMP�$
 OneWayToSource=�6�   5      �� 5      $ 
Horizontal=�6   .+�5      �� $	��HLT��6   $��12,0,0,0q�6&   �� 5       $	��MLT��6   $��55,0,0,0q�6&   �� 5!      $��BK��6   $��55,0,0,0q�6%   5"      5#      5%      5&      