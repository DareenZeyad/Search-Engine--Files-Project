using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Runtime.InteropServices;

namespace Files_Project
{
    public partial class Form1 : Form
    {
        public class FileTable
        {

            [XmlArray("Files"), XmlArrayItem(typeof(FileClass), ElementName = "FileClass")]
            public List<FileClass> FileList { get; set; }
            public FileTable()
            {
                FileList = new List<FileClass>();
            }
        }

        [XmlRoot("FileTable")]
        public class FileClass
        {
            [XmlElement(typeof(string), ElementName = "FileName")]
            public string Filename { get; set; }
            [XmlElement(typeof(string), ElementName = "Path")]
            public string path { get; set; }
        }

        public class CategoryTable
        {
            [XmlArray("Categories"), XmlArrayItem(typeof(Category), ElementName = "category")]
            public List<Category> CategoryList { get; set; }
            public CategoryTable()
            {
                CategoryList = new List<Category>();
            }
        }

        [XmlRoot("CategoryTable")]
        public class Category
        {
            [XmlElement(typeof(string), ElementName = "CatName")]
            public string CategoryName { get; set; }
            [XmlArray("Keywords"), XmlArrayItem(typeof(string), ElementName = "Keyword")]
            public List<string> Keyword { get; set; }

        }

        public class GUICategory
        {
            public string Name { get; set; }
            public string Keywords { get; set; }
        }

        public FileTable ReadFileXML()
        {
            FileTable latestFileTable = new FileTable();
            XmlDocument doc = new XmlDocument();
            doc.Load("FileName.xml");
            XmlNodeList NodeLst = doc.GetElementsByTagName("File");
            for (int i = 0; i < NodeLst.Count; ++i)
            {
                XmlNodeList children = NodeLst[i].ChildNodes;
                FileClass newFile = new FileClass();
                newFile.Filename = children[0].InnerText;
                newFile.path = children[1].InnerText;
                latestFileTable.FileList.Add(newFile);
            }
            return latestFileTable;
        }
        public CategoryTable ReadCategoryXML()
        {
            CategoryTable latestCategoryTable = new CategoryTable();
            XmlDocument doc = new XmlDocument();
            doc.Load("FileCategory.xml");
            XmlNodeList NodeLst = doc.GetElementsByTagName("category");
            for (int i = 0; i < NodeLst.Count; ++i)
            {
                XmlNodeList children = NodeLst[i].ChildNodes;
                Category newCategory = new Category();
                newCategory.Keyword = new List<string>();
                newCategory.CategoryName = children[0].InnerText;
                XmlNodeList keywords = children[1].ChildNodes;
                for (int j = 0; j < keywords.Count; ++j)
                {
                    newCategory.Keyword.Add(keywords[j].InnerText);
                }
                latestCategoryTable.CategoryList.Add(newCategory);
            }
            return latestCategoryTable;
        }

        public void WriteFileXML(string filename, string filepath)
        {
            FileTable current = ReadFileXML();
            FileClass newFile = new FileClass();
            newFile.Filename = filename;
            newFile.path = filepath;
            current.FileList.Add(newFile);
            XmlSerializer ser = new XmlSerializer(typeof(FileTable));
            using (FileStream fss = new FileStream("FileName.xml", FileMode.Create))
            {
                ser.Serialize(fss, current);
            }
        }
        public void WriteCategoryXML(string newCatName, List<string> newKeywords)
        {
            CategoryTable current = ReadCategoryXML();

            Category newCategory = new Category();
            newCategory.CategoryName = newCatName;
            newCategory.Keyword = newKeywords;
            current.CategoryList.Add(newCategory);

            XmlSerializer ser = new XmlSerializer(typeof(CategoryTable));
            using (FileStream fss = new FileStream("FileCategory.xml", FileMode.Create))
            {
                ser.Serialize(fss, current);
            }
        }

        public void RemoveFile(string filename, string filepath)
        {
            List<FileClass> currentfile = ReadFileXML().FileList;
            for (int i = 0; i < currentfile.Count; ++i)
            {
                if (currentfile[i].Filename == filename && currentfile[i].path == filepath)
                {
                    currentfile.RemoveAt(i);
                    break;
                }
            }
            FileTable newFileTable = new FileTable();
            newFileTable.FileList = currentfile;
            XmlSerializer ser = new XmlSerializer(typeof(FileTable));
            using (FileStream fs = new FileStream("FileName.xml", FileMode.Create))
            {
                ser.Serialize(fs, newFileTable);
            }
        }
        public void RemoveCategory(string catname)
        {
            List<Category> currList = ReadCategoryXML().CategoryList;
            for (int i = 0; i < currList.Count; ++i)
            {
                if (currList[i].CategoryName == catname)
                {
                    currList.RemoveAt(i);
                    break;
                }
            }
            CategoryTable newCatTable = new CategoryTable();
            newCatTable.CategoryList = currList;
            XmlSerializer ser = new XmlSerializer(typeof(CategoryTable));
            using (FileStream fs = new FileStream("FileCategory.xml", FileMode.Create))
            {
                ser.Serialize(fs, newCatTable);
            }
        }

        public void clearCategoryXML()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("FileCategory.xml");
            doc.DocumentElement.RemoveAll();
            doc.Save("FileCategory.xml");
        }
        public void clearFileXML()
        {
            XmlDocument doc2 = new XmlDocument();
            doc2.Load("FileName.xml");
            doc2.DocumentElement.RemoveAll();
            doc2.Save("FileName.xml");
        }

        public List<GUICategory> getGUICategory(List<Category> categories)
        {
            List<GUICategory> ret = new List<GUICategory>();
            for (int i = 0; i < categories.Count; ++i)
            {
                GUICategory newGUICat = new GUICategory();
                newGUICat.Name = categories[i].CategoryName;
                newGUICat.Keywords = " ";
                for (int j = 0; j < categories[i].Keyword.Count; ++j)
                {
                    newGUICat.Keywords += categories[i].Keyword[j];
                    if (j != categories[i].Keyword.Count - 1)
                        newGUICat.Keywords += " - ";
                }
                ret.Add(newGUICat);
            }
            return ret;
        }

        public void addKeyword(string catName, string keyName)
        {
            CategoryTable current = ReadCategoryXML();
            for (int i = 0; i < current.CategoryList.Count; ++i)
            {
                if (current.CategoryList[i].CategoryName == catName)
                {
                    current.CategoryList[i].Keyword.Add(keyName);
                }
            }
            XmlSerializer ser = new XmlSerializer(typeof(CategoryTable));
            using (FileStream fss = new FileStream("FileCategory.xml", FileMode.Create))
            {
                ser.Serialize(fss, current);
            }
        }
        public void DisplayKeywords(string catName)
        {
            List<Category> catList = ReadCategoryXML().CategoryList;
            for (int i = 0; i < catList.Count; ++i)
            {
                if (catList[i].CategoryName == catName)
                {
                    string keywords = "";
                    for (int j = 0; j < catList[i].Keyword.Count; ++j)
                    {
                        keywords += catList[i].Keyword[j];
                        if (j != catList[i].Keyword.Count - 1)
                            keywords += " - ";
                    }
                    MessageBox.Show(keywords, "Keywords");
                    return;
                }
            }
        }
        public void DisplayFileCategories(string fileName)
        {
            List<FileClass> fileList = ReadFileXML().FileList;
            List<string> fileCategories = new List<string>();
            string searchPath = "-1";
            for (int i = 0; i < fileList.Count; ++i)
                if (fileList[i].Filename == fileName)
                {
                    searchPath = fileList[i].path;
                    break;
                }
            searchPath += "\\" + fileName;
            FileStream fs = new FileStream(searchPath, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string fileContent = "";
            while (sr.Peek() != -1)
            {
                fileContent += sr.ReadLine();
            }
            sr.Close();
            fs.Close();
            Console.Out.WriteLine(fileContent);
            Console.Out.WriteLine(fileContent.IndexOf("JHagaMshMwgooda"));
            Console.Out.WriteLine(fileContent.IndexOf("key111"));
            List<Category> catList = ReadCategoryXML().CategoryList;
            for (int i = 0; i < catList.Count; ++i)
            {
                List<string> catKeywords = catList[i].Keyword;
                for (int j = 0; j < catKeywords.Count; ++j)
                    if (fileContent.IndexOf(catKeywords[j]) != -1)
                    {
                        fileCategories.Add(catList[i].CategoryName);
                        break;
                    }
            }
            Console.Out.WriteLine(fileCategories.Count);
            string displayCat = "";
            for (int i = 0; i < fileCategories.Count; ++i)
            {
                displayCat += fileCategories[i];
                if (i != fileCategories.Count - 1)
                    displayCat += " - ";
            }
            MessageBox.Show(displayCat, "Categories");
        }
        public void keywordsInfo(string catName)
        {
            List<FileClass> currentFiles = ReadFileXML().FileList;
            List<Category> categoryList = ReadCategoryXML().CategoryList;
            List<string> currentKeywords = new List<string>();
            for (int i = 0; i < categoryList.Count; ++i)
                if (categoryList[i].CategoryName == catName)
                {
                    currentKeywords = categoryList[i].Keyword;
                    break;
                }
            string output = "\n";
            for (int i = 0; i < currentKeywords.Count; ++i)
            {
                output += currentKeywords[i] + " :\n";
                for (int j = 0; j < currentFiles.Count; ++j)
                {
                    output += "\t" + currentFiles[j].Filename + " :\n";
                    List<string> fileContent = new List<string>();
                    FileStream fs = new FileStream(currentFiles[j].path + "\\" + currentFiles[j].Filename, FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    while (sr.Peek() != -1)
                        fileContent.Add(sr.ReadLine());
                    sr.Close();
                    fs.Close();
                    int cntr = 0;
                    for (int line = 0; line < fileContent.Count; ++line)
                    {
                        if (fileContent[line].IndexOf(currentKeywords[i]) != -1)
                        {
                            if (cntr == 0) output += "\t\tLine " + (line + 1);
                            else output += ", " + (line + 1);
                            ++cntr;
                        }
                    }
                    if (cntr != 0) output += "\n";
                    output += "\t\tCount: " + (cntr) + "\n";
                }
                //output += new string('-', 100) + "\n";
            }
            richTextBox1.Text = output;
            richTextBox1.Font = new Font("Microsoft Tai Le", 10);

            Font currentFont = richTextBox1.SelectionFont;
            FontStyle newFontStyle = (FontStyle)(currentFont.Style | FontStyle.Bold);

            for (int i = 0; i < currentKeywords.Count; ++i)
            {
                richTextBox1.Find(currentKeywords[i], 0, RichTextBoxFinds.None);
                richTextBox1.SelectionFont = new Font(currentFont.FontFamily, 11, FontStyle.Bold);
                richTextBox1.SelectionColor = Color.Blue;
            }

            //for (int i = 0; i < richTextBox1.Text.Length; ++i)
            //{
            //    1
            //    int value;
            //    string x = richTextBox1.Text[i].ToString();
            //    if (int.TryParse(x, out value)) Console.Out.WriteLine(richTextBox1.Text[i]);

            //    2
            //    if (richTextBox1.Text[i].GetType() == typeof(int)) { richTextBox1.SelectionColor = Color.Red; Console.Out.WriteLine(richTextBox1.Text[i]); }

            //    3
            //    bool flag = true;
            //    int value = (int)richTextBox1.Text[i];
            //    if (value > 57 || value < 48) flag = false;
            //    else flag = true;
            //    if (flag) { richTextBox1.SelectionColor = Color.Red; Console.Out.WriteLine(richTextBox1.Text[i]); }
            //}
        }
        public void highlightKeywords(string fileName, string catName)
        {
            string fileContent = "";
            string filePath = "-1";
            List<FileClass> currFiles = ReadFileXML().FileList;
            for (int i = 0; i < currFiles.Count; ++i)
                if (currFiles[i].Filename == fileName)
                {
                    filePath = currFiles[i].path;
                    break;
                }
            FileStream fs = new FileStream(filePath + "\\" + fileName, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            while (sr.Peek() != -1)
                fileContent += sr.ReadLine() + "\n";
            sr.Close();
            fs.Close();
            richTextBox1.Text = fileContent;
            List<Category> currCategories = ReadCategoryXML().CategoryList;
            List<string> searchKeywords = new List<string>();
            for (int i = 0; i < currCategories.Count; ++i)
            {
                if (currCategories[i].CategoryName == catName)
                {
                    searchKeywords = currCategories[i].Keyword;
                    break;
                }
            }
            for (int i = 0; i < searchKeywords.Count; ++i)
            {
                int searchIndex = 0;
                while (richTextBox1.Text.IndexOf(searchKeywords[i], searchIndex) != -1)
                {
                    richTextBox1.Find(searchKeywords[i], searchIndex, RichTextBoxFinds.None);
                    richTextBox1.SelectionBackColor = Color.Yellow;
                    searchIndex = richTextBox1.Text.IndexOf(searchKeywords[i], searchIndex) + 1;
                }
                richTextBox1.Font = new Font("Microsoft Tai Le", 11);
            }
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("You must enter Name and Path", "Error");
                return;
            }
            List<FileClass> curr = ReadFileXML().FileList;
            bool flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (curr[i].Filename == textBox1.Text && curr[i].path == textBox2.Text) flag = true;
            if (flag)
            {
                MessageBox.Show("File Already Exists", "Error");
                return;
            }
            if (textBox1.Text.IndexOf(".txt") == -1){
                MessageBox.Show("You Must Enter '.txt' extension.", "Error");
                return;
            }
            if (!File.Exists(textBox2.Text + "\\" + textBox1.Text)){
                MessageBox.Show("You Must Enter A Valid File", "Error");
                return;
            }
            WriteFileXML(textBox1.Text, textBox2.Text);
            textBox1.Clear();
            textBox2.Clear();
            MessageBox.Show("Add Successful");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("You must enter Name and Path", "Error");
                return;
            }
            List<FileClass> curr = ReadFileXML().FileList;
            bool flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (curr[i].Filename == textBox1.Text && curr[i].path == textBox2.Text) flag = true;
            if (!flag)
            {
                MessageBox.Show("No Such File", "Error");
                return;
            }
            RemoveFile(textBox1.Text, textBox2.Text);
            textBox1.Clear();
            textBox2.Clear();
            MessageBox.Show("Removed Successful");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("You must enter Category and Keyword", "Error");
                return;
            }
            List<Category> curr = ReadCategoryXML().CategoryList;
            bool flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (curr[i].CategoryName == textBox3.Text) flag = true;
            if (flag)
            {
                MessageBox.Show("Category Already Exists", "Error");
                return;
            }
            List<string> Ct = new List<string>();
            Ct.Add(textBox4.Text);
            WriteCategoryXML(textBox3.Text, Ct);
            textBox3.Clear();
            textBox4.Clear();
            MessageBox.Show("Add Successful");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("You must enter Category and Keyword", "Error");
                return;
            }
            List<Category> curr = ReadCategoryXML().CategoryList;
            bool flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (curr[i].CategoryName == textBox3.Text) flag = true;
            if (!flag)
            {
                MessageBox.Show("Category Doesn't Exist", "Error");
                return;
            }
            flag = false;
            List<Category> currList = ReadCategoryXML().CategoryList;
            for (int i = 0; i < currList.Count; ++i){
                if (curr[i].CategoryName == textBox3.Text){
                    for (int j = 0; j < curr[i].Keyword.Count; ++j){
                        if (curr[i].Keyword[j] == textBox4.Text){
                            flag = true;
                            break;
                        }
                    }
                    break;
                }
            }
            if (flag){
                MessageBox.Show("Keyword Already Exists", "Error");
                return;
            }
            addKeyword(textBox3.Text, textBox4.Text);
            textBox4.Clear();
            MessageBox.Show("Add Successful", "Warning");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = ReadFileXML().FileList;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = getGUICategory(ReadCategoryXML().CategoryList);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox5.Text == "" || textBox6.Text == ""){
                MessageBox.Show("You Must Enter Category And File Name", "Error");
                return;
            }
            List<Category> curr = ReadCategoryXML().CategoryList;
            bool flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (curr[i].CategoryName == textBox6.Text) flag = true;
            if (!flag)
            {
                MessageBox.Show("Category Doesn't Exist", "Error");
                return;
            }
            List<FileClass> currList = ReadFileXML().FileList;
            flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (currList[i].Filename == textBox5.Text) flag = true;
            if (!flag)
            {
                MessageBox.Show("File Doesn't Exist", "Error");
                return;
            }
            highlightKeywords(textBox5.Text, textBox6.Text);
            textBox7.Clear();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (textBox7.Text == "")
            {
                MessageBox.Show("You Must Enter Category Name", "Error");
                return;
            }
            List<Category> curr = ReadCategoryXML().CategoryList;
            bool flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (curr[i].CategoryName == textBox7.Text) flag = true;
            if (!flag)
            {
                MessageBox.Show("Category Doesn't Exist", "Error");
                return;
            }
            keywordsInfo(textBox7.Text);
            textBox5.Clear();
            textBox6.Clear();
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
            clearFileXML();
            MessageBox.Show("Removed All Files Successfully");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            clearCategoryXML();
            MessageBox.Show("Removed All Categories Successfully");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == ""){
                MessageBox.Show("You Must Enter Category Name", "Error");
                return;
            }
            List<Category> curr = ReadCategoryXML().CategoryList;
            bool flag = false;
            for (int i = 0; i < curr.Count; ++i)
                if (curr[i].CategoryName == textBox3.Text) flag = true;
            if (!flag)
            {
                MessageBox.Show("Category Doesn't Exist", "Error");
                return;
            }
            RemoveCategory(textBox3.Text);
            textBox3.Clear();
            textBox4.Clear();
            MessageBox.Show("Removed Successfully");
        }
    }
}
