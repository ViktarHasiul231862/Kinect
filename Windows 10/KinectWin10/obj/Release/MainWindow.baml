   M S B A M L   `   `   `  ���� F  BKinectWin10, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:KinectSetupDevKinectSetupDev      KinectSetupDev.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .localclr-namespace:KinectSetupDev     -     � Title$  Kinect Skeleton Analyst��$��750.453��$��1198.651��.��� -    �Name$ mainGrid��$��Center=�.+�� -   $ liveGrid��$
��Left=�$	��750��$	��Top=�$
��1200��.+��� -    ��Name$ startRecordingButton��$��0q�#��[" �$
��Left=�$��780,600,0,0q�$	��Top=�$��90��$��90��.���� $��Image/startRec.jpg��$
��Fill=�$��80��(� -    (�Name$ humanViewBox��$��135,42,650,180q�.���� $��UniformToFill=����� *humanSource+(� -   $ depthViewBox��$��135,42,650,180q�$y�Hidden=�.���� $��UniformToFill=����� *DepthSource+(� -   $ skeletonViewBox��$��677,42,142,180q�.���� $��UniformToFill=����� *SkeletonSource+�� -    ��Name$ switchSideCombobox��$��392,610,674,84q�$��Center=�$��56�� J� RenderTransformOrigin$ 1.254,1.017&�$��Center=�$��Center=�.@��� -	   $��Lewa strona��$
��Left=�$	��121���� -
   $��Prawa strona��$
��Left=�$	��121���� -   $��Obie strony��$
��Left=�$	��121���� -   $ uploadFilesButton��$��Wczytaj pliki��$
��Left=�$��578,610,0,84q�$	��133���� -   $ stopRecordingButton1��$��0q�#��[" �$
��Left=�$��920,600,0,0q�$	��Top=�$��90��$��90��.���� $��Image/stopRec.jpg��$
��Fill=��� -    ��Name$ statusLabel��
���  ��$
��Left=�$��
61,610,0,0q�$	��Top=�$	��128��$��56��$��Center=�$��Center=��� -   $ colorOrDepthCombobox��$
��Left=�$��234,610,0,0q�$	��Top=�$	��120��$��56��$��Center=�$��Center=�.@��� -   $��Kolorowy obraz��$
��Left=�$	��118���� -   $��Mapa głębokości��$
��Left=�$	��118��A� -    A�Name$ recordingSign��$y�Hidden=�
���  ��$
��Left=�$��42��$��1131,10,0,0q�
���   �$	��Top=�$��41��� -   $ 	movieGrid��$
��Left=�$	��710��$	��Top=�$
��1182��$��9,0,0,0q�.+��� -   $ 
backButton��$��Wstecz��$
��Left=�$��1058,656,0,0q�$	��Top=�$��95��$��38��$y�Visible=��� -   $ uploadAvi1_Button��$��Wczytaj plik ".avi"��$
��Left=�$��136,70,0,594q�$	��140���� -   $ startMovieAll�� J� Opacity$	 0.4Q�{�.  $��534,258,517,0q�$	��Top=�$��50��$��50��.���� $��Image/play.png��$
��Fill=��� -   $ startKosciec2��$	 0.4Q�{�.  $��782,588,350,0q�$	��Top=�$��50��$��50��.���� $��Image/play.png��$
��Fill=��� -   $ startKosciec1��$	 0.4Q�{�.  $��205,588,927,0q�$	��Top=�$��50��$��50��.���� $��Image/play.png��$
��Fill=�r� -   	 r�Name$	 kosciecVideoAvi1��$
��Left=�$	��325��$��136,168,0,0q�
 r� LoadedBehavior$
 Manual=�$	��Top=�$	��375��r� -   $	 kosciecVideoAvi2��$
��Left=�$	��325��$��690,168,0,0q�$
 Manual=�$	��Top=�$	��370���� -   $ labelKosciec1��$
��Left=�$��240,498,0,0q�$	��Top=�$��42��$	��152���� -   $ labelKosciec2��$�� ��$
��Left=�$��822,498,0,0q�$	��Top=�$��42��$	��149���� -   $ pauseKosciec1��$	 0.4Q�{�.  $
��Left=�$��284,588,0,0q�$	��Top=�$��50��$��50��.���� $��Image/pauza.png��$
��Fill=��� -   $ pauseKosciec2��$	 0.4Q�{�.  $
��Left=�$��862,588,0,0q�$	��Top=�$��50��$��50��.���� $��Image/pauza.png��$
��Fill=��� -   $ pauseAll��$	 0.4Q�{�.  $��534,324,517,0q�$	��Top=�$��50��$��50��.���� $��Image/pauza.png��$
��Fill=��� -    $ stopKosciec1��$	 0.4Q�{�.  $
��Left=�$��363,588,0,0q�$	��Top=�$��50��$��50��.���� $��Image/stop.png��$
��Fill=��� -!   $ stopKosciec2��$	 0.4Q�{�.  $
��Left=�$��946,588,0,0q�$	��Top=�$��50��$��50��.���� $��Image/stop.png��$
��Fill=��� -"   $ stopAll��$	 0.4Q�{�.  $��534,387,517,0q�$	��Top=�$��50��$��50��.���� $��Image/stop.png��$
��Fill=��� -#   $ uploadSkeleton1_Button��$��Wczytaj plik ".kosciec"��$
��Left=�$��337,70,0,594q�$	��140���� -$    ��Name$ kosciecVideoKosciec1��$��UniformToFill=�$
��Left=�$	��325��$��136,168,0,0q�$	��Top=�$	��375���� -%   $ kosciecVideoKosciec2��$��UniformToFill=�$
��Left=�$	��325��$��690,168,0,0q�$	��Top=�$	��370���� -&   $ readFakeSkeleton��$y�Hidden=�$#��Nagraj
przykladowy
kosciec
��$
��Left=�$��1077,10,0,0q�$	��Top=�$��95��$��56��$��Center=�$��Center=��� -'    ��Name$ speedMovie1��{�.  $
��Left=�$��22��$��322,664,0,0q� �� TextWrapping$
 Wrap=�$	��100��$	��Top=�$��52��$��Center=��� -(   $ speedLabel1��$��Szybkosc, %��$
��Left=�$��216,660,0,0q�$	��Top=�$��84���� -)   $ speedMovie2��{�.  $
��Left=�$��22��$��909,664,0,0q�$
 Wrap=�$	��100��$	��Top=�$��53��$��Center=��� -*   $ speedLabel2��$��Szybkosc, %��$
��Left=�$��804,660,0,0q�$	��Top=�$��84���� -+   $ uploadAvi2_Button��$��Wczytaj plik ".avi"��$
��Left=�$��715,70,0,594q�$	��141���� -,   $ uploadSkeleton2_Button��$��Wczytaj plik ".kosciec"��$
��Left=�$��913,70,0,594q�$	��143���� --   $ framesKosciec2��$y�Hidden=�$��0��$
��Left=�$��1022,130,0,0q�$	��Top=��� -.   $ framesKosciec1��$y�Hidden=�$��0��$
��Left=�$��473,130,0,0q�$	��Top=��� -/   $ labelOfFrame2��$y�Hidden=�$��Ramka
��$
��Left=�$��972,130,0,0q�$	��Top=�$��26���� -0   $ labelOfFrame1��$y�Hidden=�$��Ramka��$
��Left=�$��420,130,0,0q�$	��Top=�$ 0.5,0.5&��� -1    ��Name$ sliderKosciec1��{�.  $	��300��$
��Left=�$��177,540,0,0q�$	��Top=��� -2   $ sliderKosciec2��{�.  $	��300��$
��Left=�$��738,540,0,0q�$	��Top=�