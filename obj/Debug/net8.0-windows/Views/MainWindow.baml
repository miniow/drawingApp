   M S B A M L   `   `   `  ����E  AdrawingApp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null=#clr-namespace:drawingApp.ViewModelsdrawingApp.ViewModels  5   	   3clr-namespace:drawingApp.ViewsdrawingApp.Views  5   	       drawingApp.MainWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   X TFontAwesome.Sharp, Version=6.6.0.0, Culture=neutral, PublicKeyToken=d16d1e4e568ec10fFfa=http://schemas.awesome.incremented/wpf/xaml/fontawesome.sharp  5   	   3	viewModel#clr-namespace:drawingApp.ViewModels  5   	   *localclr-namespace:drawingApp.Views  5   	     � Title$  
MainWindow��5
   	   $	��450��6   $	��800��6)    � DataContext 5      )   #drawingApp.ViewModels.MainViewModel  5   
    � 	Resources 5       �� DataType)  #drawingApp.ViewModels.DrawViewModel   drawingApp.Views.DrawView(   "drawingApp.ViewModels.PpmViewModel   drawingApp.Views.PpmView��%�   (��       5      *, +)(�� 2     5      *, +)�� 5   
   "  6   .R�5        5      �� 5   
   "  6   .R�5        5      5      .��5      �  � ColumnDefinitions 5   
   �� 5      $	��150 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��35 �6   �� 5      $��* �6   .+�5!   
   ��  �� CornerRadius$ 	10,0,0,10��6   $��0��5"      $��1��6%   ��5#      �� 5$      .��5%      �    secondaryBackColor1 � Color# [  6#    � Offset$ 0Q�6P   � 5&        secondaryBackColor2# [ 6#   $ 1Q�6P   5'      ._�5)      �� .+�5+      �� $��55��5,   !   $��
15,25,0,40q�5-   !   .+�5.      �� $
��menu��6      titleColor1#��[ 5/   $   $��20(�50   $   $��
Monsterrat)�51   $   $��Medium"�52   $   $��Center=�53   $   $��0,0,0,0q�54   $   55      �� 56      .+�58      	�   
menuButton#��[ 6"    
 color1
	 � Tag#	 [ 59   "   $��0,0,0,0q�5:   "   ��5;   "   �� *ShowDrawViewCommand+.��5<      �� 
 �� Orientation$
 
Horizontal=�6%   .+�5>      !  FontAwesome.Sharp.IconImage  �  �FontAwesome.Sharp.IconImageBase`1[[FontAwesome.Sharp.IconChar, FontAwesome.Sharp, Version=6.6.0.0, Culture=neutral, PublicKeyToken=d16d1e4e568ec10f]]   Icon$ DrawPolygon=�6+     menuButtonIcon#��[ 6>   �� 5?      $
��Draw��6(     menuButtonText#��[ 64   5A      5C      	� 5D      #��[ 6"   #	 [ 5E   %   ��5F   "   �� *ShowPpmViewCommand+.��5G      �� $
 
Horizontal=�6%   .+�5H        $
 File=�6+   #��[ 67   �� 5I      $	��Ppm��6(   #��[ 63   5J      5L      5N      5O      5P      �� 5Q   
   $��0��6   $��0��6%   $
 
Horizontal=�5R   %   $��Center=�5S   %   .+�5U        $��20��5V   +   $��20��5W   +    
 color6   
Foreground# [ 5X   +   $��	35,0,10,0q�5Y   +    5U      �� *Icon+5Y   +   �� 5Z        color10#��[ 5[   (   $��16(�5\   (   $��
Monsterrat)�5]   (   $��Medium"�5^   (   $��Center=�5_   (   ��5Z      �� *	Caption+5_   (   5`      �� 5a   
   $��1��5b   %   $��1��65   $��	25,25,0,0q�5c   %   ��5a      �� *CurrentViewModel+5e      5f      5g      