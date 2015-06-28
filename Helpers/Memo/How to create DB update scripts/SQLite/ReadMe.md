How to create SQLite DB update scripts
======================================

... using SQLite Manager (0.8.3.1) FireFox plugin

First, select the table/column you want to modify, and right click as at the picture:<br />
![Step 1][Step 1]<br />

You don't have to modify the values in the appearing field, only click Change, than yes (but only once!). You have to see this:<br />
![Step 2][Step 2]<br />

Copy the change scripts from here, but don't run them with this tool! First, you have to modify parts of it, described in the picture:<br />
![Step 3][Step 3]<br />

So, now, you have the first part of the change script. Before runnig it, of course, create backups! And also create a full db export, so with all of the data, for safety sake. You can do this here: Database/Export Database<br />
Now run the update script in SQLite Manager, and create a full export also of the modified db<br />
Compare them (with a good tool). There will be triggers missing in the new one probably, and such things. Add those scripts to the update_script as well. Don't forget to check them though, eg. if you were remaining some data columns...<br />



[Step 1]: https://github.com/nvirth/Exin/blob/master/Helpers/Memo/How%20to%20create%20DB%20update%20scripts/SQLite/Step%201.png "Step 1"
[Step 2]: https://github.com/nvirth/Exin/blob/master/Helpers/Memo/How%20to%20create%20DB%20update%20scripts/SQLite/Step%202.png "Step 2"
[Step 3]: https://github.com/nvirth/Exin/blob/master/Helpers/Memo/How%20to%20create%20DB%20update%20scripts/SQLite/Step%203.png "Step 3"