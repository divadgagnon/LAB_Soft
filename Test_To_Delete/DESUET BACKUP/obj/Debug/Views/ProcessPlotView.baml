   M S B A M L   `   `   `  ����Q  MLiveCharts, Version=0.6.7.0, Culture=neutral, PublicKeyToken=d1901e6ed2eb0532;,clr-namespace:LiveCharts;assembly=LiveCharts
LiveCharts  5   	   
 LAB_V2   LAB.Views.ProcessPlotView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   0ignorehttp://www.galasoft.ch/ignore     5   	   <lvc,clr-namespace:LiveCharts;assembly=LiveCharts     5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   f bGalaSoft.MvvmLight.Platform, Version=5.2.0.37226, Culture=neutral, PublicKeyToken=5f873c45e98af8a1*cmd http://www.galasoft.ch/mvvmlight  5	   	     � DataContext  5   	   �� *ProcessPlot+   Locator �� Source# [   � 	Resources 5       �� MergedDictionaries �� Source�� 5   
    5      �� 5      $+ %/LAB_V2;component/Skins/MainSkin.xaml?�6%   %    5      .��5      � 
������ .+�5         LiveCharts.LineChart  $
��auto��5      $
��auto��5        MenuBackground#���  5      ���	#FF9E0000���	#FF1109BD$��Stretch=�5      $��Stretch=�5      $��16(�5      %   LiveCharts.CoreComponents.Chart   DisableAnimations .    	Hoverable .  $��0,0,0,0q�5!         Series 5      �� *
DataSeries+   AxisX 5$         LiveCharts.Axis  	   Title$	 Time (minutes)��6   
   MaxValue
 66   �� *
MaxValue+   MinValue 6T   �� *
MinValue+   LabelFormatter 6r   �� *
XFormatter+   AxisY 5'        $	 Temperature (°C)��6   $	
 100X�68   $ 0X�6G   5)      5*      5+      