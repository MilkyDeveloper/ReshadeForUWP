   M S B A M L   `   `   `  ����D  @FluentWPF, Version=0.8.0.6, Culture=neutral, PublicKeyToken=nullP6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPFSourceChord.FluentWPF  5   	   H DReshadeForUWP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:ReshadeForUWPReshadeForUWP 5   	      ReshadeForUWP.MainWindow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Efw6clr-namespace:SourceChord.FluentWPF;assembly=FluentWPF     5   	   -localclr-namespace:ReshadeForUWP     5   	   D @ModernWpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM IModernWpf.Controls, Version=0.9.3.0, Culture=neutral, PublicKeyToken=null,ui!http://schemas.modernwpf.com/2019   63     � Title$  Reshade for UWP��5	   	   $	��450��6!   $	��800��6.   .��5      � .+�5   
   �� $��0,40,0,0q�6   $��Reshade for UWP��6&   $��Center=�6=   $	��Top=�6Z   $��30(�6r   �� 5   
    �� Orientation$ 
Horizontal=�6   $��Center=�6.   $	��Top=�6K   $��	0,100,0,0q�6c   .+�5      �� $��Center=�6   $��10q�63   $��Global User Interface)�6?   	Step 1.6k   �� 5      -   5         ��Name$ chooseAppButton��5      $��Choose a game to launch��6.   $��35��6P   &   ModernWpf.Controls.FlyoutService   Flyout 5        ModernWpf.Controls.Flyout  5      -   5        Flyout1��   Content. 5      �� .+�5      �� -   5         ��Name$ listbox1��5   &   $	��350��68   $	��150��6D   $��0,10,0,0q�6Q   6e   5      5      5      �� 5       $��5q�6   $��?��6    $��Center=�6,   $
��Bold"�6G   $��16(�6Y   {�.   ^� ShowOnDisabled .  ^� ShowDuration$ 99999��6�    ^� BetweenShowDelay$ 0��6�   $��35��6�   	 � ToolTip	 5!      �� 5"      .+�5#      �� $
��Bold"�6$   $��14(�66   $��0,0,0,5q�6D   Finding your app name6l   �� 5$      971. Press the Choose an app to launch button on the left�� 5&      2. Scroll down to your game�� 5(      3. Click it!5*      �� 5+      
�������$��0,1,0,0q�66   $	��0,8q�6P   � 5,      .+�5-      �� $
��Wrap=�6(   $��Italic$�6<   ��Tip: You can press any random entry and then type in the first character of your game's publisher and then it will jump to the app names starting with that letter. E.g. You can press "B" for Bethesda (Doom Eternal) and it will jump there.6?  5.      5/      51      53      �� 56   
   $ 
Horizontal=�6   $��Center=�6.   $	��Top=�6K   $��	0,150,0,0q�6c   .+�58      �� $��Center=�6   $��0,0,10,0q�63   	Step 2.6N   �� 5:      -   5        $ chooseprocessNameButton��5:      $E��?Launch your game from the start menu and then press this button��66   $��35��6�    5;        5<      . 5=      �� .+�5>      �� ?=Launch your game and select the name of it in the list below.6g   �� 5?      -   5        $ listbox3��5?   &   $	��450��68   $	��250��6D   $��0,10,0,0q�6Q   6e   �� 5@      -   5        $ 
doneButton��5@   3   $
��0,10q�6%   $"��Done filling out everything?��6G   $��35��6n   6z   5A      5B      5D      5F      �� 5I   
   $ 
Horizontal=�6   $��Center=�6.   $��Bottom=�6K   $��261,0,259.6,40q�6f   .+�5L      �� -   5        $��Generate Reshade.ini��5L      $��35��64   $��10q�6[    5M        5N      . 5O      �� .+�5P      �� -   5        
 ��Name$ 
 generateReshadeIniProgress��5P   (   $��0,0,0,0q�6L   Done!6d   5Q      5R      5T      �� 5V      -	   5        $ otherDataButton��5V      $��
Other Data��6.   $��35��6C   $��10q�6O    5W        5X      . 5Y      �� $ 
Horizontal=�6%   $
��1000��6>   .+�5Z      �� $ Vertical=�6)   .+�5[   "   �� -
   5         ��Name$ uwpPackageFamilyTextbox��5[   *   
������ $��0q�6d    �� 
IsReadOnly .  �� TextWrapping$
 Wrap=�6�   .��6�   UWP Package Family Name:6�   �� 5\   "   -   5        $ uwpPackageFullTextbox��5\   *   
������ $��0q�6b    . $
 Wrap=�6�   .��6�   UWP Package Full Name:6�   �� 5]   "   -   5        $  uwpPackageDirectoryTextbox��5]   *   
������ $��0q�6g    . $
 Wrap=�6�   .��6�   !UWP Package Directory Location:6�   �� 5^   "   -   5        $! appxManifestLocationTextbox��5^   *   
������ $��0q�6h    . $
 Wrap=�6�   .��6�   AppxManifest.xml location:6�   5_      �� 5`      $ Vertical=�6)   .+�5a   "   �� $��10q�6,   UWP Package Full Name:6Q   �� 5b   "   $��10q�6,   UWP Package Directory Name:6V   �� 5c   "   $��10q�6,   Appxmanifest location:6Q   5d      5e      5f      5h      5j      �� 5k   
   $��358,277,357.6,0q�6   $��Extras��6-   $��Center=�6;   $	��Top=�6X   $��30(�6p   5m      5n      