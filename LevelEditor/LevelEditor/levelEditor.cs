/* 
 * Level Editor
 * -------------------------
 * By Matthew Heroux
 * Copyright 2012
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;

namespace LevelEditor
{
    public partial class levelEditor : Form
    {
        // globals
        public string appName = "Level Editor v0.12 BETA";
        public string tabWarning = "Load valid *.map file and Parse Code to view table.";
        public string fileChosen = "";
        public string fileDir = AppDomain.CurrentDomain.BaseDirectory + @"GAME\MQBtG\System\maps\";
        public string tilesetDir = AppDomain.CurrentDomain.BaseDirectory + @"GAME\MQBtG\System\tilesets\";
        public string backgroundDir = AppDomain.CurrentDomain.BaseDirectory + @"GAME\MQBtG\System\backgrounds\";
        public string objectDir = AppDomain.CurrentDomain.BaseDirectory + @"GAME\MQBtG\System\objects\";
        public string tilesetSelection = "0.png";
        public string copyrightText = "\u00a9" + DateTime.Now.Year + " Matthew Heroux";

        // levelEditor load
        public levelEditor()
        {
            InitializeComponent();

            // sets applicaiton name
            this.Text = appName;

            // sets properties, save, temp, grid, objects, tiles, characters tab warnings
            tabPropertiesWarning.Text = tabWarning;
            tabSaveWarning.Text = tabWarning;
            tabTempWarning.Text = tabWarning;
            tabGridWarning.Text = tabWarning;
            tabObjectsWarning.Text = tabWarning;
            tabTilesWarning.Text = tabWarning;
            tabCharactersWarning.Text = tabWarning;

            // Configure Tab Tileset 
            tilesetPic.ImageLocation = tilesetDir + tilesetSelection;
            foreach (String file in System.IO.Directory.GetFiles(tilesetDir))
            {
                cBoxTileset.Items.Add(new System.IO.FileInfo(file).Name);
            }
        }

        // load, save, saveAs, new file funcions
        public void file(string parameter)
        {
            switch (parameter)
            {
                case "new":
                    // Creates new file
                    fileChosen = fileDir + "new.map";
                    codeBox.LoadFile(fileChosen, RichTextBoxStreamType.PlainText);

                    // Sets Window Name
                    this.Text = appName + " " + Path.GetFileName(fileChosen);

                    // Change Text for Save Name
                    menuSave.Text = "Save " + Path.GetFileName(fileChosen);
                    menuSaveAs.Text = "Save " + Path.GetFileName(fileChosen) + " As...";
                    break;
                case "load":
                    // Loads Code
                    loadMapDialog.InitialDirectory = fileDir;
                    loadMapDialog.Title = "Select a map file to load";
                    loadMapDialog.FileName = "";
                    loadMapDialog.Filter = "Map Files|*.map";

                    if (loadMapDialog.ShowDialog() != DialogResult.Cancel)
                    {
                        fileChosen = loadMapDialog.FileName;
                        codeBox.LoadFile(fileChosen, RichTextBoxStreamType.PlainText);

                        // Sets Window Name
                        this.Text = appName + " " + Path.GetFileName(fileChosen);

                        // Change Text for Save Name
                        menuSave.Text = "Save " + Path.GetFileName(fileChosen);
                        menuSaveAs.Text = "Save " + Path.GetFileName(fileChosen) + " As...";
                    }
                    break;
                case "save":
                    // Saves Code
                    try
                    {
                        File.WriteAllText(fileChosen, codeBox.Text);
                        MessageBox.Show("Save", "Save successful");
                    }
                    catch
                    {
                        MessageBox.Show("Error");
                    }
                    break;
                case "saveAs":
                    // Saves file
                    SaveFileDialog saveFile1 = new SaveFileDialog();
                    saveFile1.DefaultExt = "*.map";
                    saveFile1.Filter = "Map Files|*.map";
                    if (saveFile1.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFile1.FileName.Length > 0)
                    {
                        // Save code
                        codeBox.SaveFile(saveFile1.FileName, RichTextBoxStreamType.PlainText);

                        // Change Text for Save Name
                        menuSave.Text = "Save " + Path.GetFileName(saveFile1.FileName);
                        menuSaveAs.Text = "Save " + Path.GetFileName(saveFile1.FileName) + " As...";
                    }
                    break;
            }
        }

        // convert LuaTable into DataTables
        public DataTable convertLuaTable(string tableName)
        {
            // define DataTable.
            DataTable table = new DataTable();
            // get first appearence of array counts out expecting 3 spaces " = " to { bracket
            string data = codeBox.Text;
            int first = data.IndexOf(tableName) + tableName.Length + 3;
            int count = first;
            string luaTableCurrentChar;
            int luaTableDimensionalChecker = 0;

            switch (tableName)
            {
                case "fieldMap:main()":
                    table.Columns.Add("code", typeof(string));
                    first = data.IndexOf(tableName) + tableName.Length;
                    table.Rows.Add(data.Substring(first, data.Length - first - 3));
                    break;
                case "fieldMap.grid":
                    table.Columns.Add("Level", typeof(int));
                    table.Columns.Add("Row", typeof(int));
                    table.Columns.Add("Element", typeof(string));

                    int level = 0;
                    int row = 0;
                    int element = 0;
                    int elementValue;
                    string elementContents = "";

                    // Parses luaTable to dataTable
                    do
                    {
                        // converts string to Char to run check
                        luaTableCurrentChar = data[count].ToString();
                        // counts brackets to get table start and finish point
                        if (luaTableCurrentChar == "{") { luaTableDimensionalChecker++; }
                        else if (luaTableCurrentChar == "}") { luaTableDimensionalChecker--; }
                        //loads into data array
                        bool canConvert = int.TryParse(luaTableCurrentChar, out elementValue);
                        // do nothing
                        if (luaTableCurrentChar == " " || luaTableCurrentChar == "/n") { }
                        // add new level
                        else if (luaTableDimensionalChecker == 2 && luaTableCurrentChar == "{") { level++; row = 0; }
                        else if (luaTableDimensionalChecker == 3 && luaTableCurrentChar == "{") { row++; }
                        // add element contents to table
                        else if (luaTableDimensionalChecker == 2 && luaTableCurrentChar == "}") { table.Rows.Add(level, row, elementContents); elementContents = ""; }
                        // add new element counter++
                        else if (luaTableCurrentChar == "," && luaTableDimensionalChecker == 3) { element++; }
                        // handles negitive values
                        else if (luaTableCurrentChar == "-") { elementContents += luaTableCurrentChar; }
                        // new element
                        else if (canConvert) { elementContents += elementValue + ", "; }
                        count++;
                    } while (luaTableDimensionalChecker != 0);
                    break;

                default:
                    table.Columns.Add("ID", typeof(string));
                    table.Columns.Add("Value", typeof(string));
                    first = data.IndexOf(tableName) + tableName.Length + 3;

                    // Parses luaTable to dataTable
                    do
                    {
                        // converts string to Char to run check
                        luaTableCurrentChar = data[count].ToString();
                        // counts brackets to get table start and finish point
                        if (luaTableCurrentChar == "{") { luaTableDimensionalChecker++; }
                        else if (luaTableCurrentChar == "}") { luaTableDimensionalChecker--; }
                        count++;
                    } while (luaTableDimensionalChecker != 0);

                    string[] words = data.Substring(first, count - first).Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                    foreach (string word in words)
                    {
                        string[] elements = word.Split('=');
                        for (int i = 1; i <= elements.Length - 1; i++)
                        {
                            // adds content every other = sign.
                            if (i % 2 != 0) { table.Rows.Add(elements[i - 1], elements[i]); }
                            // Selects tileset loaded
                            if (i == 1) { cBoxTileset.Text = elements[i].Substring(2, 1) + ".png"; }
                        }
                    }
                    break;
            }
            return table;
        }

        // parse code function
        public string parseCode()
        {
            try
            {
                // loads properties, save, temp, grid, objects, tiles, characters into dataGrids
                dataGridProperties.DataSource = convertLuaTable("fieldMap.properties");
                dataGridSave.DataSource = convertLuaTable("fieldMap.save");
                dataGridTemp.DataSource = convertLuaTable("fieldMap.temp");
                dataGridGrid.DataSource = convertLuaTable("fieldMap.grid");
                dataGridObjects.DataSource = convertLuaTable("fieldMap.objects");
                dataGridTiles.DataSource = convertLuaTable("fieldMap.tiles");
                dataGridCharacters.DataSource = convertLuaTable("fieldMap.characters");
                dataGridMain.DataSource = convertLuaTable("fieldMap:main()");

                // removes properties, save, temp, grid, objects, tiles, characters tab warnings 
                tabPropertiesWarning.Visible = false;
                tabSaveWarning.Visible = false;
                tabTempWarning.Visible = false;
                tabGridWarning.Visible = false;
                tabTilesWarning.Visible = false;
                tabObjectsWarning.Visible = false;
                tabCharactersWarning.Visible = false;
                tabMainWarning.Visible = false;

                // display success message
                return ("Parse Code was successful.");
            }
            catch (Exception errorCode)
            {
                // sets properties, save, temp, grid, objects, tiles, characters tab warnings
                tabWarning = "PARSE CODE FAILED - Select valid *.map file.";
                tabPropertiesWarning.Text = tabWarning;
                tabSaveWarning.Text = tabWarning;
                tabTempWarning.Text = tabWarning;
                tabGridWarning.Text = tabWarning;
                tabObjectsWarning.Text = tabWarning;
                tabTilesWarning.Text = tabWarning;
                tabCharactersWarning.Text = tabWarning;
                tabMainWarning.Text = tabWarning;
                tabPropertiesWarning.Visible = true;
                tabSaveWarning.Visible = true;
                tabTempWarning.Visible = true;
                tabGridWarning.Visible = true;
                tabTilesWarning.Visible = true;
                tabObjectsWarning.Visible = true;
                tabCharactersWarning.Visible = true;
                tabMainWarning.Visible = true;

                return (tabWarning + "\n\n[" + errorCode.ToString() + "]");
            }
        }


        // gets tile from cropped tileset 
        private static Image getTileImage(Image img, int x, int y)
        {
            // set default tile size
            int width = 46;
            int height = 36;
            x = x * width - width;
            y = y * height - height;

            // corps image using rectangle
            Rectangle tileSize = new Rectangle(x, y, width, height);
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(tileSize, bmpImage.PixelFormat);

            return (Image)(bmpCrop);
        }

        // gets object from cropped tileset 
        private static Image getObjectImage(Image objectName, int x, int y)
        {
            return (objectName);
        }

        // generate map function
        private string genMap(string sImageText)
        {
            try
            {
                // set working images
                Image tilesetImage = Image.FromFile(tilesetDir + tilesetSelection);
                string backgroundSelected = dataGridProperties[1, 1].Value.ToString().Trim('"', ',').Substring(2) + ".png";
                Bitmap backgroundImage = (Bitmap)Image.FromFile(backgroundDir + backgroundSelected);

                // make graphics object
                Graphics objGraphics = Graphics.FromImage(backgroundImage);

                // sets maximum occurence of rows and columns to determine scrollbar sizes
                int maxRows = 0;
                int maxColumns = 0;

                // for each row
                for (int v = 0; v < dataGridGrid.Rows.Count - 1; v++)
                {
                    // for each column
                    string[] rowContents = dataGridGrid[2, v].Value.ToString().Split(',');
                    for (int i = 0; i < rowContents.Length; i++)
                    {
                        // OBJECTS ARE - VALUES in grid
                        if (rowContents[i].Contains("-"))
                        {
                            // for each element - search for tile in dataGridObjects
                            for (int b = 0; b <= dataGridObjects.Rows.Count - 1; b++)
                            {
                                try
                                {
                                    // get dataGridObjects from dataGridGrid
                                    string selectedRow = dataGridObjects[0, b].Value.ToString().Trim(' ', '[', ']');
                                    if (rowContents[i].Contains(selectedRow))
                                    {
                                        // x and y locations of tilesets 0 1
                                        int objectX = 1;// Convert.ToInt32(dataGridTiles[1, b].Value.ToString().Substring(2, 1));
                                        int objectY = 2;// Convert.ToInt32(dataGridTiles[1, b].Value.ToString().Substring(4, 1));

                                        // gets name of object file
                                        string objectName = "";
                                        Regex r = new Regex("\"[\\w ]*\"");
                                        MatchCollection mc = r.Matches(dataGridObjects[1, b].Value.ToString());
                                        foreach (Match m in mc)
                                        {
                                            objectName = m.Value.Substring(1, m.Value.Length - 2);
                                        }

                                        Image objectImage = Image.FromFile(objectDir + objectName + ".png");
                                        Image drawObject = getObjectImage(objectImage, objectX, objectY);
                                        objGraphics.DrawImage(drawObject, new Point(i * 18 - v * 18, i * 9 - drawObject.Height + 30 + v * 9));
                                        break; // object found
                                    }
                                }
                                catch { }
                            }
                        }
                        // TILES ARE + VALUES in grid
                        else
                        {
                            // for each element - search for tile in dataGridTile
                            for (int b = 0; b <= dataGridTiles.Rows.Count - 1; b++)
                            {
                                try
                                {
                                    // get dataGridTiles from dataGridGrid
                                    string selectedRow = dataGridTiles[0, b].Value.ToString().Trim(' ', '[', ']');
                                    if (rowContents[i].Contains(selectedRow))
                                    {
                                        // x and y locations of tilesets 0 1
                                        int tilesetX = Convert.ToInt32(dataGridTiles[1, b].Value.ToString().Substring(2, 1));
                                        int tilesetY = Convert.ToInt32(dataGridTiles[1, b].Value.ToString().Substring(4, 1));
                                        Image drawTile = getTileImage(tilesetImage, tilesetX, tilesetY);
                                        objGraphics.DrawImage(drawTile, new Point(i * 18 - v * 18, i * 9 + v * 9));
                                        break; // tile found
                                    }
                                }
                                catch { }
                            }
                        }
                        // Used to detrmine ScrollBar Size
                        if (i > maxColumns) { maxColumns = i; };
                    }
                    // Used to detrmine ScrollBar Size
                    if (v > maxColumns) { maxColumns = v; };
                }

                // draws water mark on map
                Font watermarkFont = new Font("Arial", 18, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                objGraphics.DrawString(copyrightText, watermarkFont, new SolidBrush(Color.FromArgb(255, 255, 255)), 280, 250);

                picMap.Image = backgroundImage;

                // set scrollbars
                xScrollImage.Minimum = 480;
                xScrollImage.Maximum = maxColumns;
                yScrollImage.Maximum = 272; 
                yScrollImage.Maximum = maxRows;


                return ("Generate Map was successful.");
            }
            catch (Exception errorCode)
            {
                return ("Generate Map failed." + "\n\n[" + errorCode.ToString() + "]");
            }

        }

        // menu items
        private void menuNew_Click(object sender, EventArgs e) { file("new"); }
        private void menuLoad_Click(object sender, EventArgs e) { file("load"); }
        private void menuSave_Click(object sender, EventArgs e) { file("save"); }
        private void menuSaveAs_Click(object sender, EventArgs e) { file("saveAs"); }
        private void menuExit_Click(object sender, EventArgs e) { this.Close(); }

        // tool items
        private void toolNew_Click(object sender, EventArgs e) { file("new"); }
        private void toolSave_Click(object sender, EventArgs e) { file("save"); }
        private void toolParseCode_Click(object sender, EventArgs e) { MessageBox.Show(parseCode(), ""); }
        private void toolGenMap_Click(object sender, EventArgs e) { MessageBox.Show(genMap(tilesetSelection)); }

        // change tileset selection
        private void cBoxTileset_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Change Tileset
            tilesetSelection = cBoxTileset.Text;
            tilesetPic.ImageLocation = tilesetDir + tilesetSelection;
        }

        // execute app to modify tileset
        private void btEdit_Click(object sender, EventArgs e)
        {
            Process editFile;
            editFile = Process.Start(tilesetDir + tilesetSelection);
            //myProc.CloseMainWindow();
        }

        // adjust background color of tileset menu 
        private void cBoxBackgroundColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            // adjust background color of tileset
            System.Drawing.ColorConverter colConvert = new ColorConverter();
            tabTileset.BackColor = (System.Drawing.Color)colConvert.ConvertFromString(cBoxBackgroundColor.Text);
        }

        private void menuAbout_Click(object sender, EventArgs e) { MessageBox.Show(appName + "\nBy Matthew Heroux\n-----------------\n\n This is a level editor for my isometric game engine current written in Lua for the PSP. \n\n This version is rather limited in features including some basic functions. The next release will include better object support, more complex parsing functions, character support, scrollbar support, and collision support.\n\nThanks for testing this BETA.\n\n All work is copyright all rights reserved."); }
        private void menuHelp_Click(object sender, EventArgs e) { MessageBox.Show("HELP FILES UNAVALIBLE\n-----------------\n\n*Load a file or create a new file\n*Press Parse Code\n*Press Generate Map"); }

        private void loadMapDialog_FileOk(object sender, CancelEventArgs e)
        {
        }
        private void levelEditor_Load(object sender, EventArgs e)
        {
        }

    }
}
