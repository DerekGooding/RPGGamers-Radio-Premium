   M S B A M L   `   `   `  ���� F  BRadio Leech, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullM+clr-namespace:Radio_Leech.View.UserControlsRadio_Leech.View.UserControls  )clr-namespace:Radio_LeechRadio_Leech  I)clr-namespace:Radio_Leech.View.ConvertersRadio_Leech.View.Converters  =#clr-namespace:Radio_Leech.ViewModelRadio_Leech.ViewModel      Radio_Leech.MainWindow   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     :uc+clr-namespace:Radio_Leech.View.UserControls     +localclr-namespace:Radio_Leech     7c)clr-namespace:Radio_Leech.View.Converters     2vm#clr-namespace:Radio_Leech.ViewModel       � Title$  RPG Radio Premium��$	��650��$	��810�� � 	Resources 2   ,Radio_Leech.View.Converters.PausePlayConvert   boolToPlayConvert#   Radio_Leech.ViewModel.LeechVM  vm��%   &	        &	          .���� -    ��Name$ MyCanvas��
������ .+�]� ���	#FF424242���� *ActualWidth+ �� ElementName$ MyCanvas������ *ActualHeight+$ MyCanvas��.+�o� $	��Top=�.@�m� $
��File��.@�m� $��Scan for Songs Online�� � ToolTip$O ILooks for working URL links one at a time. Will not add duplicate entries�� m� Command �� *SearchLinksCommand+m� $��Download��$) #Download the currently playing song�� �� *DownloadCommand+m� -   $��Show Downloads Path��$0 *Tells you the path files are downloaded to��m� $��Reload Database�� �� *DownloadAllCommand+�� m� -   $
��Exit��m� -   $��ViewMenu��$
��View��m� $
��Help��.@�m� -   $��About��$( "Information about this application���� m� -   $��GitHub��$, &Navigate to this applications git repo��m� -   $��Original Radio Site��$1 +Navigate to the original RPG Radio web site���� $��Bottom=�.@��� ���	#FF424242.����  �� Orientation$ 
Horizontal=�  �� 
TargetType �� Property	 �� Value
 �� 
TargetType ��Name �� Triggers P� Property P� Value �� 
TargetName��%�  '��       '�� %     �� " ��.���  � ��
	 ������� " ��.���  � ��$	 10,0,0,0q���  � ��$	 35����  � ��$	 Center=���  � ��$	 Center=���  � ��
	 �������  � ��
	 ���� ��  � ��$	 0q���  � ��$	 20(���  � ��	 �� "
 ��.R��� $ Border��#��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver$
 True��. ��� $ Border�� � ��	 �	#7FFFFFFF.+��� $
��|◀������ *PreviousCommand+�� ���� *	IsPlaying+ �� 	Converter# [  ���� *PauseCommand+�� $
��▶|������ *NextCommand+�� $��15,0,5,0q�
�������$��2q��� ���� *
Duration+�� $��10���� ���� *Status+�� $��🔈🔉🔊��$y�Hidden=�� $��Bottom=�$��50�����	#FF424242.+��� -    ��Name$ MyWaveImage��$
��Left=�$	��800������ *WaveFormSource+�� ���	#7F000000$
��Left=� J� IsHitTestVisible$ False������ *	FillWidth+�� $	��Top=�$ 
Horizontal=�$��3q����	#FF424242 ��%^   '��       '�� %     �� " ��.���  � ��
	 ������� " ��.���  � ��
	 �����.+�� .+��� -	    ��Name$ 
SearchText��$	��300��$��Center=�$! Filter songs by typing here�����	#FF383838
�������$��20(����� *Search+ �� UpdateSourceTrigger$ PropertyChanged=��� $ False��$��🔍 Search Songs��$��Center=�$
��Left=�$��2,2,0,0q�$
��Bold"�
�������$��11(� J� Opacity$	 0.5Q����� " ��.���  � y�$	 	Collapsed=� �� Triggers ��  �� Binding �� *Text+$ 
SearchText�� �� Value  .a���  � y�$	 Visible=��� $	��100������ *	SongCount+�� $
��🔊��$��16(���  � Minimum$ 0Q� � Maximum$	 0.5Q�$	��100��$��Center=� �� IsSnapToTickEnabled$
 True�� �� TickFrequency$ 0.005Q� � Value �� *Volume+�� $
��💾��$��30��
������ $��0q����� *SaveVolumeCommand+r� -
    r�Name$ MyPlayer��$
��Left=� r� LoadedBehavior$ Manual=��� -     ��Name$  MainTabControl��$	��Top=����	#FF424242 ��%�   '��       '�� +     '�� P     �� " ��.���  � ��	 �	#FF2E2E2E�� " ��.���  � ��
	 ������� " ��.���  � ��	 �	#FF383838��  � ��
	 �����.@��� $
��Main��.��]� .+��� -   ! �� SelectionMode$! Single=����	#FF1A1A1A���� *FilteredSongs+" �� SelectedItem" �� *SelectedSong+���� .R�/   )Radio_Leech.View.UserControls.SongControl  #   Song# �� �� $��Custom Playlists��.��]� 
���iii�.+�� .+��� $
��Left=����	#FF1A1A1A$��2q�$	��300��$! Single=����� *	Playlists+" �� *SelectedPlaylist+���� .R�3   -Radio_Leech.View.UserControls.PlaylistControl  $   Playlist$ �� �� $��Coming soon��$��32(��� ���	#FF1A1A1A$��2q�$! Single=����� *FilteredSongs+" �� *SelectedSong+���� .R�  # �� �� $��Twitch��.��]� ���	#FF1A1A1A.+��� $	��Top=� ��%?  '��       �� " ��.���  � ��$	 Center=���  � ��$	 Center=���  � ��
	 �������  � ��
	 ���� ��  � ��$	 0q���  � ��	 �� "
 ��.R��� $ Border��#��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver$
 True��. ��� $ Border�� � ��	 �	#7FFFFFFF.+��� $ 
Horizontal=�$��10q�.+��� $��Center=�% d� 	IsChecked% �� *IsRequesting+�� $!��Enable Twitch Interactivity���� $ 
Horizontal=�.+��� $��Pending Song Requests��$��0,20,0,0q��� $��Clear��$��10,0,0,0q�$	��100��$��20��$��Bottom=�$��15(����� *ClearRequestsCommand+�� ���	#FF1A1A1A$! Single=����� *RequestedSongs+���� .R�  # �� �� $��Settings��${�False��.��]� ���	#FF1A1A1A.+��� .+��� $��	Dark Mode���� $	��Top=�$
 True��$ 1Q�$ 0Q�$ 1Q�]� �� -   $��MyBorder��$��2,0,2,2q�
���   �
�������& �� CornerRadius$& 	0,0,10,10��
' �� Top' �� *PopupHeight+ $   Radio_Leech.HalfValueConverter  	HalfValue��%   &	         ( �� Left( `� ) `� 	Converter#) [ .1��� $ MyCanvas��* �� Path$* ActualWidth��� $ MyBorder��$* ActualWidth�._�� $	��200��.+��� $��Center=����� *	PopupText+���� *PopupFontSize+