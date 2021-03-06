**FlashCardsX**
==============

**Motivation for the project**
--------------

My motivation for this project was the Think101 edX course(https://www.youtube.com/user/xTHINK101) which covers a chapter about effective learning. In this course it was discussed that the most successful learning strategies include interleaving, retrieval and distributed practices. However, even though it has been proved that these are better strategies for learning than highlighting, rereading or cramming, they are not used because there are no readily available, easy to use tools to apply these strategies to our learning methods. 

Flash cards are very simple yet effective tools for interleaving, retrieval and distributed learning. Many people underestimate their effectiveness or are too lazy to create their own flashcards even though they are very powerful tools for retrieval practise which helps to memorise information for longer times. As a channel factor I tried to create an easy way to make flash card decks and make them easily accessible. By implementing cloud storage and PDF exportation, these flash cards can be used on any PC, smart phone or e-reader we have. Hopefully this will help people learn more effectively and easily.

My other purpose was to practise coding in C# and the usage of WPF and the MVVM design pattern. I understand my code may have many mistakes or suboptimal codes so feel free to message me about possible fixes as I am eager to learn more.

WARNING: If you use Internet Explorer, this app clears all cookies when using cloud services.

**The structure of the program**
--------------

The file format of the decks: <Deck Name>_<TopicA Name>_<TopicB Name>.xml I store these data in the file name as well as in the XML file so that the decks don't need to be deserialised in order to get these data. My client only recognises files with this syntax. All XML files will be listed by the program, but only the files with the correct syntax can be opened. The decks are serialised using XMLWriter so they can be deserialised easily on any platform. 

To allow logging out of cloud services I had to clear every cookie from IE. (Note: I had to use unsafe code here) SkyDrive had the option of retrieving a logoutUrl, but navigating there did not result in logging out. Even if we did not persist our session, IE still remembered our session and when trying to log in, it only asked for permissions. Dropbox allowed us to log in as other user after app restart, however I wanted to allow the user to switch users runtime.

For syncing purposes I used log files. One which logs the deletion date of files. During each sync the app downloads the latest deletion log from the cloud, if the entry in the log is newer than the file the app has, then the file gets deleted. After that we download the newer deck files, then upload
them if there are any. To check modification date, I use SkyDrive files' description property and on Dropbox a Changelog. This was necessary since modifying the modification date was not possible.

**Possible improvements for the project**
--------------

-Implement a way to jump to card in deck view.
-Implement a way to jump to card from testing to edit it.
-Dynamic difficulty assesment. Based on stats. Maybe make it optional.
-Better editor for card text
-Clients for other platforms

**Issues:**
--------------

-Need to clean up MainViewModel. Maybe implement a separate Dropbox and SkyDrive service class.
-Random activity log error
-Big cards load slowly in deck view

**Compile:**
--------------

For this project I used VS2013. In order to use the Dropbox API you have to request an App folder type API key from https://www.dropbox.com/developers. For the OneDrive API you need a key from http://msdn.microsoft.com/onedrive/ . Then you have to put these keys inside a Keys.txt file inside the Debug/Release folder.
The content and format of Keys.txt has to be the following (without the brackets):
[OneDriveClientID]
[DropboxAppKey]
[DropboxAppSecret]