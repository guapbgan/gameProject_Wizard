using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace Wizard
{
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }
        public Form getSubForm()
        {
            Form form = new Form() {
                FormBorderStyle = FormBorderStyle.None,
                ControlBox = false,
                AutoScroll = false,
                StartPosition = FormStartPosition.CenterParent,
                Font = new Font("微軟正黑體",9)
            };
            form.Size = new Size(1260, 677);
            form.LayoutMdi(MdiLayout.ArrangeIcons);
            form.MdiParent = this;
            return form;
        }
        private void gameForm_Load(object sender, EventArgs e)
        {
            HomePage homePage = new HomePage(getSubForm(), this);
            

        }
        class HomePage
        {
            Form form;
            GameForm gameForm;
            public HomePage(Form form, GameForm gameForm)
            {
                form.BackgroundImage = Image.FromFile(@"material/mainPage.png");
                this.form = form;
                this.gameForm = gameForm;
                Show();
            }
            public void Show()
            {
                void exit(object senderE, EventArgs eE)
                {
                    Application.Exit();
                }
                form.Controls.Clear();
                Control[] objList = new Control[3];
                objList[0] = new Button() { Size = new Size(230, 50), Location = new Point(510, 389), Text = "新遊戲" , BackColor = Color.White };
                objList[1] = new Button() { Size = new Size(230, 50), Location = new Point(510, 475), Text = "讀取遊戲", BackColor = Color.White };
                objList[2] = new Button() { Size = new Size(230, 50), Location = new Point(510, 561), Text = "離開", BackColor = Color.White };
                //NewGame newGame = new NewGame(form);
                ((Button)objList[0]).Click += (object sender, EventArgs e) => 
                {
                    NewGame newGame = new NewGame(gameForm.getSubForm(), gameForm);
                    form.Dispose();
                };
                //LoadGame loadGame = new LoadGame(form);
                ((Button)objList[1]).Click += (object sender, EventArgs e) => 
                {
                    LoadGame loadGame = new LoadGame(gameForm.getSubForm(), gameForm);
                    form.Dispose();
                };

                ((Button)objList[2]).Click += exit;
                foreach (Control obj in objList)
                    form.Controls.Add(obj);
                form.Show();
                form.Location = new Point(0, 0);
            }
            
        }
        class NewGame
        {
            Form form;
            GameForm gameForm;
            EnterName enterName;
            DataSet ds;
            bool flag;
            string playerID;
            public NewGame(Form form,GameForm gameForm)
            {
                this.gameForm = gameForm;
                this.form = form;
                Show();
            }
            public string ReadName(EnterName form)
            {
                return form.name;
            }
            public void ResetName(EnterName form)
            {
                form.name = "";
            }
            public void Show()
            {
                flag = true;
                while (true)
                {
                    enterName = new EnterName();
                    enterName.ShowDialog();
                    playerID = ReadName(enterName);
                    ds = LoadGame.LoadData("SELECT * FROM PlayerData where PlayerID ='" + playerID.Replace("'", "''") + "'");
                    if(ds.Tables[0].Rows.Count != 0)
                    {
                        MessageBox.Show("ID已存在");
                        Show();
                        return;
                    }
                    else
                    {
                        LoadGame.SaveDateEdit("insert into PlayerData(PlayerID) values('" + playerID.Replace("'", "''") + "')");
                        ds = LoadGame.LoadData("SELECT * FROM PlayerData where PlayerID ='" + playerID.Replace("'", "''") + "'");
                        EnterPrepare();
                        break;
                    }

                }
            }
            public void EnterPrepare()
            {
                Prepare prepare = new Prepare(ds, gameForm.getSubForm(), gameForm);
                form.Dispose();
            }
        }

        class LoadGame
        {
            Form form;
            GameForm gameForm;
            DataGridView dataView;
            public LoadGame(Form form, GameForm gameForm)
            {
                this.gameForm = gameForm;
                form.BackgroundImage = Image.FromFile(@"material/mainPage.png");
                this.form = form;
                Refresh();
            }
            public static void SaveDateEdit(string sqlstr)
            {
                try
                {
                    SqlConnection cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;" +
                                                            "AttachDbFilename=|DataDirectory|save.mdf;" +
                                                            "Integrated Security=True");
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(sqlstr, cn);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            public static DataSet LoadData(string sqlstr)
            {
                SqlConnection cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;" +
                                "AttachDbFilename=|DataDirectory|save.mdf;" +
                                "Integrated Security=True");
                SqlDataAdapter da = new SqlDataAdapter(sqlstr, cn);
                DataSet ds = new DataSet();
                da.Fill(ds);
                return ds;
            }
            public void Initialize(object sender, EventArgs e)
            {
                Refresh();

            }
            public void Refresh()
            {
                form.Controls.Clear();
                Control[] objList = new Control[3];
                dataView = new DataGridView()
                {
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToOrderColumns = false,
                    Location = new Point(540,200),
                    Size = new Size(200, 347),
                    ScrollBars = ScrollBars.Vertical,
                    ReadOnly = true,
                    AllowUserToResizeColumns = false,
                    AllowUserToResizeRows = false,
                    RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders
                    //Dock = DockStyle.Fill
                };
                Button BtnSelect = new Button() { Size = new Size(140, 50), Location = new Point(434, 577), Text = "選擇紀錄", BackColor = Color.White };
                Button BtnDelete = new Button() { Size = new Size(140, 50), Location = new Point(672, 577), Text = "刪除紀錄", BackColor = Color.White };
                BtnSelect.Click += Select;
                BtnDelete.Click += Delete;
                DataSet ds = LoadData("SELECT PlayerID AS 玩家名稱, Level AS 等級 FROM PlayerData");
                dataView.DataSource = ds.Tables[0];

                objList[0] = dataView;
                objList[1] = BtnSelect;
                objList[2] = BtnDelete;
                foreach (Control obj in objList)
                    form.Controls.Add(obj);
                form.Show();
                form.Location = new Point(0, 0);
            }

            void Select(object sender, EventArgs e)
            {
                string playerId = dataView.CurrentRow.Cells[0].Value.ToString();
                DataSet ds = LoadData("SELECT * FROM PlayerData where PlayerID ='" + playerId.Replace("'", "''") + "'");
                Prepare prepare = new Prepare(ds, gameForm.getSubForm(), gameForm);
                form.Dispose();

            }
            void Delete(object sender, EventArgs e)
            {
                string playerId = dataView.CurrentRow.Cells[0].Value.ToString();
                SaveDateEdit("DELETE FROM PlayerData where PlayerID = '" + playerId.Replace("'", "''") + "'");
                Refresh();
            }
        }

        class Prepare
        {
            string playerID;
            ComboBox difficultSelect;
            public Dictionary<string, int> values;
            Dictionary<string, int> valuesBase = new Dictionary<string, int>()
                    {
                        { "HP", 100},
                        { "Stamina", 100},
                        { "Sword Attack", 10},
                        { "Magic Attack", 10},
                        { "Defend", 3},
                        { "Heal", 10},
                        { "Power Up", 3},
                    };
            Form form;
            GameForm gameForm;
            public Prepare(DataSet ds, Form form, GameForm gameForm)
            {
                playerID = ds.Tables[0].Rows[0]["playerID"].ToString();
                values = new Dictionary<string, int>()
                {
                    { "level", int.Parse(ds.Tables[0].Rows[0]["level"].ToString())},
                    { "skillPoint", int.Parse(ds.Tables[0].Rows[0]["skillPoint"].ToString())},
                    { "HP", int.Parse(ds.Tables[0].Rows[0]["hp"].ToString())},
                    { "Stamina", int.Parse(ds.Tables[0].Rows[0]["stamina"].ToString())},
                    { "Sword Attack", int.Parse(ds.Tables[0].Rows[0]["swordAttackValue"].ToString())},
                    { "Magic Attack", int.Parse(ds.Tables[0].Rows[0]["magicAttackValue"].ToString())},
                    { "Defend", int.Parse(ds.Tables[0].Rows[0]["defendValue"].ToString())},
                    { "Heal", int.Parse(ds.Tables[0].Rows[0]["healValue"].ToString())},
                    { "Power Up", int.Parse(ds.Tables[0].Rows[0]["powerUpValue"].ToString())},

                };
                this.form = form;
                form.BackColor = Color.FromArgb(242, 217, 217);
                //form.BackgroundImage = Image.FromFile(@"material/preparePage.png");
                this.gameForm = gameForm;
                Show();
            }
            public Prepare(string playerID,Dictionary<string, int> values, Form form, GameForm gameForm)
            {
                this.values = values;
                this.form = form;
                this.gameForm = gameForm;
                this.playerID = playerID;
                Show();
            }
            private void CheckAndCost(string str)
            {

                int cost = int.Parse(CalculateCost(str));
                if (values["skillPoint"] >= cost)
                {
                    if (str == "Defend" || str == "Power Up")
                        values[str]++;
                    else
                        values[str] += valuesBase[str];
                    values["level"]++;
                    values["skillPoint"] -= cost;

                    Show();
                }
                else
                    MessageBox.Show("技能點數不足");
            }
            private string CalculateCost(string str)
            {
                if (str == "Defend" || str == "Power Up")
                    return (values[str] - valuesBase[str] + 1).ToString();
                else
                    return (values[str] / valuesBase[str]).ToString();
            }
            private void upgradeHp(object sender, EventArgs e)
            {
                CheckAndCost("HP");
            }
            private void upgradeStamina(object sender, EventArgs e)
            {
                CheckAndCost("Stamina");
            }
            private void upgradeSwordAttack(object sender, EventArgs e)
            {
                CheckAndCost("Sword Attack");
            }
            private void upgradeMagicAttack(object sender, EventArgs e)
            {
                CheckAndCost("Magic Attack");
            }
            private void upgradeDefend(object sender, EventArgs e)
            {
                CheckAndCost("Defend");
            }
            private void upgradeHeal(object sender, EventArgs e)
            {
                CheckAndCost("Heal");
            }
            private void upgradePowerUp(object sender, EventArgs e)
            {
                CheckAndCost("Power Up");
            }
            private void Fight(object sender, EventArgs e)
            {
                Fight fight = new Fight(playerID, values, difficultSelect.Text, gameForm.getSubForm(), gameForm);
                form.Dispose();
            }
            private void Save(object sender, EventArgs e)
            {
                //string playerID, Dictionary< string, int> values
                LoadGame.SaveDateEdit("UPDATE PlayerData SET Level = " + values["level"] +
                                      ", hp = " + values["HP"] +
                                      ", Stamina = " + values["Stamina"] +
                                      ", SwordAttackValue = " + values["Sword Attack"] +
                                      ", MagicAttackValue = " + values["Magic Attack"] +
                                      ", DefendValue = " + values["Defend"] +
                                      ", HealValue = " + values["Heal"] +
                                      ", PowerUpValue = " + values["Power Up"] +
                                      ", SkillPoint = " + values["skillPoint"] +
                                      " WHERE PlayerID = '" + playerID.Replace("'", "''") + "'");
                MessageBox.Show("成功存檔");
            }
            private void Exit(object sender, EventArgs e)
            {
                HomePage homePage = new HomePage(gameForm.getSubForm(),gameForm);
                form.Dispose();
            }
            public void Show()
            {

                form.Controls.Clear();
                form.BackColor = Color.FromArgb(242, 217, 217);
                string[] tagStrList = new string[] { "Player", "Level", "Skill Point", "HP", "Stamina", "Sword Attack", "Magic Attack", "Defend", "Heal", "Power Up" };
                int[] x = new int[] { 146, 525, 913 };
                int[] y = new int[] { 73, 142, 211, 280, 349 };


                Dictionary<string, int[]> tagLocation = new Dictionary<string, int[]>()
                {
                    { tagStrList[0], new int[]{ x[0], y[0]} },
                    { tagStrList[1], new int[]{ x[0], y[1]} },
                    { tagStrList[2], new int[]{ x[0], y[2]} },
                    { tagStrList[3], new int[]{ x[1], y[0]} },
                    { tagStrList[4], new int[]{ x[1], y[2]} },
                    { tagStrList[5], new int[]{ x[2], y[0]} },
                    { tagStrList[6], new int[]{ x[2], y[1]} },
                    { tagStrList[7], new int[]{ x[2], y[2]} },
                    { tagStrList[8], new int[]{ x[2], y[3]} },
                    { tagStrList[9], new int[]{ x[2], y[4]} },
                };
                Dictionary<string, string> valueDict = new Dictionary<string, string>()
                {
                    { tagStrList[0], playerID },
                    { tagStrList[1], values["level"].ToString() },
                    { tagStrList[2], values["skillPoint"].ToString() },
                    { tagStrList[3], values["HP"].ToString() },
                    { tagStrList[4], values["Stamina"].ToString() },
                    { tagStrList[5], values["Sword Attack"].ToString() },
                    { tagStrList[6], values["Magic Attack"].ToString() },
                    { tagStrList[7], values["Defend"].ToString() },
                    { tagStrList[8], values["Heal"].ToString() },
                    { tagStrList[9], values["Power Up"].ToString()}
                };

                int count = 0, btnCount = 0;
                Label[] tagList = new Label[tagStrList.Length], valueList = new Label[tagStrList.Length];
                Button[] upgradeBtnList = new Button[tagList.Length - 3];
                int[] offset = new int[] { 20, 20 };
                foreach (string str in tagStrList)
                {
                    tagList[count] = new Label()
                    {
                        Text = str,
                        Location = new Point(tagLocation[str][0], tagLocation[str][1]),
                        Size = new Size(200, 20),
                        Font = new Font("微軟正黑體",12, FontStyle.Bold),
                        BackColor = Color.Transparent
                        //BorderStyle = BorderStyle.FixedSingle
                    };

                    valueList[count] = new Label() {
                        Text = valueDict[str],
                        Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1]),
                        Font = new Font("微軟正黑體",9, FontStyle.Bold),
                        Size = new Size(200, 15),
                        ForeColor = Color.Red,
                        BackColor = Color.Transparent
                    };

                    switch (str)
                    {
                        case "HP":
                            upgradeBtnList[btnCount] = new Button() {
                                Text = "需要 " + CalculateCost(str) + "點技能點數以升級",
                                Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1] * 2),
                                Size = new Size(150, 25),
                                BackColor = Color.Transparent};
                            upgradeBtnList[btnCount].Click += upgradeHp;
                            btnCount++;
                            break;
                        case "Stamina":
                            upgradeBtnList[btnCount] = new Button() { Text = "需要 " + CalculateCost(str) + "點技能點數以升級", Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1] * 2), Size = new Size(150, 25), BackColor = Color.Transparent };
                            upgradeBtnList[btnCount].Click += upgradeStamina;
                            btnCount++;
                            break;
                        case "Sword Attack":
                            upgradeBtnList[btnCount] = new Button() { Text = "需要 " + CalculateCost(str) + "點技能點數以升級", Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1] * 2), Size = new Size(150, 25), BackColor = Color.Transparent };
                            upgradeBtnList[btnCount].Click += upgradeSwordAttack;
                            btnCount++;
                            break;
                        case "Magic Attack":
                            upgradeBtnList[btnCount] = new Button() { Text = "需要 " + CalculateCost(str) + "點技能點數以升級", Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1] * 2), Size = new Size(150, 25), BackColor = Color.Transparent };
                            upgradeBtnList[btnCount].Click += upgradeMagicAttack;
                            btnCount++;
                            break;
                        case "Defend":
                            upgradeBtnList[btnCount] = new Button() { Text = "需要 " + CalculateCost(str) + "點技能點數以升級", Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1] * 2), Size = new Size(150, 25), BackColor = Color.Transparent };
                            upgradeBtnList[btnCount].Click += upgradeDefend;
                            btnCount++;
                            break;
                        case "Heal":
                            upgradeBtnList[btnCount] = new Button() { Text = "需要 " + CalculateCost(str) + "點技能點數以升級", Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1] * 2), Size = new Size(150, 25), BackColor = Color.Transparent };
                            upgradeBtnList[btnCount].Click += upgradeHeal;
                            btnCount++;
                            break;
                        case "Power Up":
                            upgradeBtnList[btnCount] = new Button() { Text = "需要 " + CalculateCost(str) + "點技能點數以升級", Location = new Point(tagLocation[str][0] + offset[0], tagLocation[str][1] + offset[1] * 2), Size = new Size(150, 25), BackColor = Color.Transparent };
                            upgradeBtnList[btnCount].Click += upgradePowerUp;
                            btnCount++;
                            break;
                    }
                    count++;
                }
                foreach (Label obj in tagList)
                {
                    form.Controls.Add(obj);
                }
                foreach (Label obj in valueList)
                {
                    form.Controls.Add(obj);
                }
                foreach (Button obj in upgradeBtnList)
                {
                    form.Controls.Add(obj);
                }
                Button[] btnList = new Button[3];
                btnList[0] = new Button() { Size = new Size(230, 50), Location = new Point(460, 389), Text = "開始戰鬥", BackColor = Color.White };
                btnList[0].Click += Fight;
                btnList[1] = new Button() { Size = new Size(230, 50), Location = new Point(460, 475), Text = "存檔", BackColor = Color.White };
                btnList[1].Click += Save;
                btnList[2] = new Button() { Size = new Size(230, 50), Location = new Point(460, 561), Text = "離開", BackColor = Color.White };
                btnList[2].Click += Exit;
                foreach (Button obj in btnList)
                {
                    form.Controls.Add(obj);
                }

                ComboBox difficultSelect = new ComboBox()
                {
                    Location = new Point(btnList[0].Left + btnList[0].Width + 20, btnList[0].Top + (btnList[0].Height * 1 / 2)),
                    DropDownStyle = ComboBoxStyle.DropDownList,
                };
                difficultSelect.Items.Add("困難");
                difficultSelect.Items.Add("普通");
                difficultSelect.Items.Add("簡單");
                difficultSelect.SelectedIndex = 1;
                Label difficultTag = new Label() { Location = new Point(difficultSelect.Left, btnList[0].Top), Text = "難度：", BackColor = Color.Transparent };
                form.Controls.Add(difficultSelect);
                form.Controls.Add(difficultTag);
                this.difficultSelect = difficultSelect;
                form.Show();
                form.Location = new Point(0, 0);
                

            }

        }
        class Fight
        {

            class Player
            {
                Dictionary<string, int> values;
                PictureBox pictureBox, pictureBoxOri = new PictureBox();
                PictureBox effectPowerUp, effectShield;
                ProgressBar progressBarHp, progressBarStamina;
                Label labelMessage, labelStamina, labelHp;
                Form form;
                public Timer timerStamina;
                int maxHp;
                int maxStamina;
                int _currentHp;
                public int currentHp
                {
                    get
                    {
                        return _currentHp;
                    }
                    set
                    {
                        int ori_currentHp = _currentHp;
                        if (value <= 0)
                            _currentHp = 0;
                        else
                        {
                            if (value > maxHp)
                                _currentHp = maxHp;
                            else
                                _currentHp = value;
                        }
                        progressBarHp.Value = _currentHp;
                        labelHp.Text = "HP: " + _currentHp.ToString() + "/" + maxHp.ToString();
                        if (ori_currentHp > value)
                            Injury(pictureBox);
                    }
                }
                int _currentStamina;
                int currentStamina
                {
                    get { return _currentStamina; }
                    set
                    {
                        if (value <= 0)
                            _currentStamina = 0;
                        else
                        {
                            if (value > maxStamina)
                                _currentStamina = maxStamina;
                            else
                                _currentStamina = value;
                            progressBarStamina.Value = _currentStamina;
                            labelStamina.Text = "體力： " +  _currentStamina.ToString() + "/" + maxStamina.ToString();

                        }
                    }
                }
                Dictionary<string, int> staminaCostDict = new Dictionary<string, int>
                    {
                        {"Sword Attack",15 },
                        {"Magic Attack",15 },
                        {"Heal",15 },
                        {"Defend",20 },
                        {"Power Up",25 },
                    };
                public bool shield = false;
                bool buff = false;
                int labelMessageOriX, labelMessageOriY;
                public Player(
                    Dictionary<string, int> values, 
                    PictureBox pictureBox, PictureBox effectPowerUp, PictureBox effectShield,
                    ProgressBar progressBarHp, ProgressBar progressBarStamina, 
                    Label labelMessage, Label labelHp, Label labelStamina, 
                    Form form)
                {
                    this.values = values;
                    this.form = form;
                    effectPowerUp.Location = pictureBox.Location;
                    effectShield.Location = pictureBox.Location;
                    this.effectPowerUp = effectPowerUp;
                    this.effectShield = effectShield;
                    _currentHp = values["HP"];
                    maxHp = currentHp;
                    _currentStamina = values["Stamina"];
                    maxStamina = currentStamina;

                    timerStamina = new Timer() { Interval = 200 };
                    timerStamina.Tick += (object sender, EventArgs e) =>
                    {
                        currentStamina += 1;
                    };
                    timerStamina.Start();

                    progressBarHp.Maximum = maxHp;
                    progressBarHp.Value = maxHp;

                    progressBarStamina.Maximum = maxStamina;
                    progressBarStamina.Value = maxStamina;

                    labelMessageOriX = labelMessage.Left;
                    labelMessageOriY = labelMessage.Top;

                    labelHp.Text = "HP: " +maxHp.ToString() + "/" + maxHp.ToString();
                    labelStamina.Text = maxStamina + "/" + maxStamina.ToString();
                    this.labelStamina = labelStamina;
                    this.labelHp = labelHp;
                    this.pictureBox = pictureBox;
                    pictureBoxOri.Image = pictureBox.Image;
                    this.progressBarHp = progressBarHp;
                    this.progressBarStamina = progressBarStamina;
                    this.labelMessage = labelMessage;
                }
                public static void Injury(PictureBox pictureBox)
                {
                    Timer timer = new Timer() { Interval = 200 };
                    int startSign = 0, stopSign = 5;
                    timer.Tick += (object sender, EventArgs e) =>
                    {
                        startSign++;
                        pictureBox.Visible = !(pictureBox.Visible);
                        if(startSign == stopSign)
                        {
                            pictureBox.Visible = true;
                            timer.Dispose();
                        }
                    };
                    timer.Start();
                    
                }
                public static void Movement(PictureBox pictureBox, bool direction)
                {
                    Timer timer = new Timer() { Interval = 1 };
                    int offset = 10, sign, startSign = 0, stopSign = 1; ;
                    if (direction)
                        sign = 1;
                    else
                        sign = -1;
                     
                    timer.Tick += (object sender, EventArgs e) =>
                    {
                        if (startSign == stopSign)
                        {
                            pictureBox.Location = new Point(pictureBox.Left - offset * sign, pictureBox.Top - offset * -sign);
                            timer.Dispose();
                            return;
                        }
                        timer.Interval = 200;
                        pictureBox.Location = new Point(pictureBox.Left + offset * sign, pictureBox.Top + offset * -sign);
                        startSign++;
                    };
                    timer.Start();
                }
                public bool checkStamina(string skillName)
                {
                    if (staminaCostDict[skillName] > currentStamina)
                        return false;
                    else
                    {
                        currentStamina -= staminaCostDict[skillName];
                        return true;
                    }
                }
                private Label callLabelMessage()
                {
                    Label message = new Label() { Size = labelMessage.Size, Location = labelMessage.Location ,Visible = labelMessage.Visible}; 
                    message.Visible = true;
                    int stopSign = 20, startSign = 0;
                    Timer timer = new Timer() { Interval = 100 };
                    timer.Tick += (object sender2, EventArgs e2) =>
                    {
                        message.Top -= 2;
                        startSign++;
                        if (startSign == stopSign)
                        {
                            message.Visible = false;
                            message.Text = "";
                            timer.Enabled = false;
                            message.Dispose();
                            timer.Dispose();
                        }
                    };
                    timer.Start();
                    form.Controls.Add(message);
                    return message;
                }
                public void SwordAttack(Emeny emeny)
                {
                    string skillName = "Sword Attack";
                    string skillOutput = "使用劍進行攻擊，";
                    if (checkStamina(skillName))
                    {
                        int damage = values[skillName];
                        if (buff == true)
                            damage *= 2;
                        if (emeny.shield == "sword")
                        {
                            damage = (int)(damage * 0.2);
                            emeny.currentHp -= damage;
                            callLabelMessage().Text = skillOutput + "被擋下來！對敵人造成" + damage.ToString() + "點傷害";
                        }
                        else
                        {
                            emeny.currentHp -= damage;
                            callLabelMessage().Text = skillOutput + "對敵人造成" + damage.ToString() + "點傷害";

                        }
                        Movement(pictureBox, true);
                    }
                    else
                    {
                        callLabelMessage().Text = "無法" + skillOutput + "體力不足" + staminaCostDict[skillName].ToString() + "點";
                    }
                }
                public void MagicAttack(Emeny emeny)
                {
                    string skillName = "Magic Attack";
                    string skillOutput = "使用魔法進行攻擊，";
                    if (checkStamina(skillName))
                    {
                        int damage = values[skillName];
                        if (buff == true)
                            damage *= 2;
                        if (emeny.shield == "magic")
                        {
                            damage = (int)(damage * 0.2);
                            emeny.currentHp -= damage;
                            callLabelMessage().Text = skillOutput + "被擋下來！對敵人造成" + damage.ToString() + "點傷害";
                        }

                        else
                        {
                            emeny.currentHp -= damage;
                            callLabelMessage().Text = skillOutput + "對敵人造成" + damage.ToString() + "點傷害";

                        }
                        Movement(pictureBox, true);
                    }
                    else
                    {
                        callLabelMessage().Text = "無法" + skillOutput + "體力不足" + staminaCostDict[skillName].ToString() + "點";
                    }
                }
                public void Heal()
                {
                    string skillName = "Heal";
                    string skillOutput = "使用治癒，";
                    if (checkStamina(skillName))
                    {
                        int point = values[skillName];
                        if (buff == true)
                            point *= 2;
                        currentHp += point;
                        callLabelMessage().Text = skillOutput + "生命恢復" + point.ToString() + "點";
                        Movement(pictureBox, true);
                    }
                    else
                        callLabelMessage().Text = "無法" + skillOutput + "體力不足" + staminaCostDict[skillName].ToString() + "點";
                }
                public void Defend()
                {
                    string skillName = "Defend";
                    string skillOutput = "使用護盾，";
                    if (checkStamina(skillName))
                    {
                        int second = values[skillName];
                        Timer timer = new Timer() { Interval = second * 1000 };
                        timer.Tick += (object sender, EventArgs e) =>
                        {
                            ((Timer)sender).Stop();
                            shield = false;
                            pictureBox.Image = pictureBoxOri.Image;
                        };
                        timer.Start();
                        pictureBox.Image = effectShield.Image;
                        buff = false;
                        shield = true;
                        callLabelMessage().Text = "替自己上了" + second.ToString() + "秒的護盾";
                        Movement(pictureBox, true);
                    }
                    else
                        callLabelMessage().Text = "無法" + skillOutput + "體力不足" + staminaCostDict[skillName].ToString() + "點";
                }
                public void PowerUp()
                {
                    string skillName = "Power Up";
                    string skillOutput = "使用效果增強，";
                    if (checkStamina(skillName))
                    {
                        int second = values[skillName];

                        Timer timer = new Timer() { Interval = second * 1000 };
                        timer.Tick += (object sender, EventArgs e) =>
                        {
                            ((Timer)sender).Stop();
                            buff = false;
                            pictureBox.Image = pictureBoxOri.Image;

                        };
                        timer.Start();
                        pictureBox.Image = effectPowerUp.Image;
                        shield = false;
                        buff = true;
                        callLabelMessage().Text = "替自己上了" + second.ToString() + "秒的效果增強";
                        Movement(pictureBox, true);
                    }
                    else
                        callLabelMessage().Text = "無法" + skillOutput + "體力不足" + staminaCostDict[skillName].ToString() + "點";
                }
                public void DoNotThing()
                {
                    callLabelMessage().Text = "...???";
                }
            }
            class Emeny
            {
                Dictionary<string, int> values;
                PictureBox pictureBox, effectSwordSheild, effectMagicSheild;
                ProgressBar progressBarHp;
                Label labelMessage;
                Label labelHp;
                Random rnd = new Random();
                string difficultLevel;
                public string shield;
                int maxHp;
                int atk;
                int _currentHp;
                public int currentHp
                {
                    get
                    {
                        return _currentHp;
                    }
                    set
                    {
                        if (value <= 0)
                            _currentHp = 0;
                        else
                        {
                            if (value > maxHp)
                                _currentHp = maxHp;
                            else
                                _currentHp = value;

                        }
                        progressBarHp.Value = _currentHp;
                        labelHp.Text = "HP: " + _currentHp.ToString() + "/" + maxHp.ToString();
                        Player.Injury(pictureBox);

                    }
                }
                public Emeny(Dictionary<string, int> values, 
                             PictureBox pictureBox, PictureBox effectSwordSheild, PictureBox effectMagicSheild,
                             ProgressBar progressBarHp, 
                             Label labelMessage, Label labelHp, 
                             string difficultLevel)
                {
                    shield = "sword";
                    int[] parameter = setEnemyLevel(values, difficultLevel);
                    atk = parameter[0];
                    maxHp = parameter[1];
                    _currentHp = maxHp;
                    this.difficultLevel = difficultLevel;
                    this.values = values;
                    this.pictureBox = pictureBox;
                    this.effectMagicSheild = effectMagicSheild;
                    this.effectSwordSheild = effectSwordSheild;
                    pictureBox.Image = effectSwordSheild.Image;
                    progressBarHp.Maximum = maxHp;
                    progressBarHp.Value = maxHp;
                    this.progressBarHp = progressBarHp;
                    this.labelMessage = labelMessage;
                    labelMessage.TextChanged += (object sender, EventArgs e) =>
                    {
                        if (labelMessage.Text != "")
                        {
                            labelMessage.Visible = true;
                            int oriX = labelMessage.Left, oriY = labelMessage.Top;
                            int stopSign = 20, startSign = 0;
                            Timer timer = new Timer() { Interval = 100 };
                            timer.Tick += (object sender2, EventArgs e2) =>
                            {
                                labelMessage.Top += 2;
                                startSign++;
                                if (startSign == stopSign)
                                {
                                    labelMessage.Visible = false;
                                    labelMessage.Text = "";
                                    labelMessage.Location = new Point(oriX, oriY);
                                    timer.Enabled = false;
                                    timer.Dispose();
                                }
                            };
                            timer.Start();
                        }
                    };
                    this.labelHp = labelHp;
                    labelHp.Text = maxHp.ToString() + "/" + maxHp.ToString();

                }
                public void Attack(Player player)
                {
                    if (player.shield == false)
                    {
                        player.currentHp -= atk;
                        labelMessage.Text = "敵人對你造成" + atk + "點傷害";
                    }
                    else
                    {
                        player.currentHp -= (int)(atk * 0.2);
                        labelMessage.Text = "護盾效果，敵人對你造成少量的" + (int)(atk * 0.2) + "點傷害";
                    }
                    Player.Movement(pictureBox, false);
                }
                public void ChangeShield()
                {
                    if (shield == "sword")
                    {
                        shield = "magic";
                        pictureBox.Image =  effectMagicSheild.Image;
                    }
                    else
                    {
                        shield = "sword";
                        pictureBox.Image =  effectSwordSheild.Image;
                    }
                    labelMessage.Text = "敵人轉換盾牌屬性";
                    Player.Movement(pictureBox, false);
                }
                private int[] setEnemyLevel(Dictionary<string, int> values, string difficultLevel)
                {
                    int[] parameter = new int[2];
                    float coff = (float)Math.Pow(1.08f, values["level"]);
                    if (difficultLevel == "困難")
                    {
                        parameter[0] = (int)(10 * coff*1.5);
                        parameter[1] = (int)(100 * coff*1.5);
                    }
                    else if(difficultLevel == "普通")
                    {
                        parameter[0] = (int)(10 * coff);
                        parameter[1] = (int)(100 * coff);
                    }
                    else
                    {
                        parameter[0] = (int)(10 * coff * 0.5);
                        parameter[1] = (int)(100 * coff * 0.5);
                    }
                    return parameter;
                }

            }
            Form form;
            Random rnd = new Random();
            Timer emenyTimer;
            Panel pan;
            BPN bpn;
            Dictionary<string, int> values;
            ProgressBar progressBarPlayer, progressBarEnemy, progressBarStamina;
            Label labelMessagePlayer, labelMessageEnemy, labelHpPlayer, labelHpEmeny, labelStamina;
            PictureBox pictureBoxPlayer, pictureBoxEnemy;
            Player player;
            Emeny emeny;
            GameForm gameForm;
            string difficultLevel;
            string playerID;
            public Fight(string playerID,Dictionary<string, int> values, string difficultLevel, Form form,  GameForm gameForm)
            {
                
                this.form = form;
                form.BackColor = Color.FromArgb(242, 217, 217);
                this.values = values;
                this.gameForm = gameForm;
                this.playerID = playerID;
                this.difficultLevel = difficultLevel;
                form.KeyUp += (object sender, KeyEventArgs e) =>
                {
                    if(e.KeyCode == Keys.Space)
                        PlayerAct();
                };
                form.Controls.Clear();
                bpn = new BPN(@"material/w12", @"material/w23", @"material/w34");
                progressBarEnemy = new ProgressBar() { Size = new Size(448, 40), Location = new Point(575, 12) };
                progressBarPlayer = new ProgressBar() { Size = new Size(448, 40), Location = new Point(804, 629) };
                progressBarStamina = new ProgressBar() { Size = new Size(500, 20), Location = new Point(50, 575) };
                pan = new Panel(form, 50, 50);
                labelMessagePlayer = new Label() { Size = new Size(300, 16), Visible = false, Location = new Point(575, 433) };
                labelMessageEnemy = new Label() { Size = new Size(300, 16), Visible = false, Location = new Point(1048, 232) };
                labelHpEmeny = new Label() { Location = new Point(progressBarEnemy.Left, progressBarEnemy.Top + progressBarEnemy.Height+10) };
                labelHpPlayer = new Label() { Location = new Point(progressBarPlayer.Left, progressBarPlayer.Top - labelHpEmeny.Height) };
                labelStamina = new Label() { Location = new Point(progressBarStamina.Left, progressBarStamina.Top + progressBarStamina.Height) };
                pictureBoxEnemy = new PictureBox() {
                    Location = new Point(1048, 12),
                    Size = new Size(200, 200),
                    Image = Image.FromFile(@"material/enemy_wizard.png"),
                };
                pictureBoxPlayer = new PictureBox() {
                    Location = new Point(575, 469),
                    Size = new Size(200, 200),
                    Image = Image.FromFile(@"material/player_wizard.png")
                };
                PictureBox effectPowerUp = new PictureBox()
                {
                    Size = pictureBoxPlayer.Size,
                    Image = Image.FromFile(@"material/player_powerUp.png"),
                    Visible = false,
                    
                };
                PictureBox effectShield = new PictureBox()
                {
                    Size = pictureBoxPlayer.Size,
                    Image = Image.FromFile(@"material/player_shield.png"),
                    Visible = false
                };
                PictureBox effectSwordSheild = new PictureBox()
                {
                    Size = pictureBoxPlayer.Size,
                    Image = Image.FromFile(@"material/enemy_shield_sword.png"),
                    Visible = false
                };
                PictureBox effectMagicSheild = new PictureBox()
                {
                    Size = pictureBoxPlayer.Size,
                    Image = Image.FromFile(@"material/enemy_shield_magic.png"),
                    Visible = false
                };
                Control[] objList = new Control[15];
                Button btnExit = new Button() { Location = new Point(10, 10), Size = new Size(80, 25), BackColor = Color.White, Text = "離開戰鬥" , TabStop = false};
                btnExit.Click += (object sender, EventArgs e) =>
                {
                    Exit();
                };
                objList[0] = progressBarEnemy;
                objList[1] = progressBarPlayer;
                objList[2] = progressBarStamina;
                objList[3] = pictureBoxPlayer;
                objList[4] = pictureBoxEnemy;
                objList[5] = labelMessageEnemy;
                objList[6] = labelMessagePlayer;
                objList[7] = labelHpPlayer;
                objList[8] = labelHpEmeny;
                objList[9] = labelStamina;
                objList[10] = effectPowerUp;
                objList[11] = effectShield;
                objList[12] = effectSwordSheild;
                objList[13] = effectMagicSheild;
                objList[14] = btnExit;
                foreach (Control obj in objList)
                {
                    form.Controls.Add(obj);
                }
                player = new Player(
                    values, 
                    pictureBoxPlayer,
                    effectPowerUp,
                    effectShield,
                    progressBarPlayer, progressBarStamina, labelMessagePlayer, 
                    labelHpPlayer,labelStamina, 
                    form);
                emeny = new Emeny(values, pictureBoxEnemy, effectSwordSheild, effectMagicSheild, progressBarEnemy, labelMessageEnemy, labelHpEmeny, difficultLevel);
                emenyTimer = new Timer() { Interval = 5000 };
                emenyTimer.Tick += (object sender, EventArgs e) =>
                {
                    EmenyAct();
                };
                form.Show();
                form.Location = new Point(0, 0);
                emenyTimer.Start();



            }

            public void PlayerAct()
            {
                float[,] result = bpn.Feedforward(pan.Read());
                float maxProb = 0;
                int skillClass = -1;
                for (int i = 0; i < result.GetLength(0); i++)
                {
                    if (result[i, 0] > 0.7 && result[i, 0] > maxProb)
                    {
                        skillClass = i;
                        maxProb = result[i, 0];
                    }
                }
                switch (skillClass)
                {
                    case 0:
                        player.SwordAttack(emeny);
                        break;
                    case 1:
                        player.Defend();
                        break;
                    case 2:
                        player.Heal();
                        break;
                    case 3:
                        player.PowerUp();
                        break;
                    case 4:
                        player.MagicAttack(emeny);
                        break;
                    default:
                        player.DoNotThing();
                        break;
                }
                pan.Initialize();
                End(player.currentHp, emeny.currentHp);
            }
            public void EmenyAct()
            {
                int dice = rnd.Next(0, 3);
                if (dice <= 1)
                    emeny.Attack(player);
                else
                    emeny.ChangeShield();
                End(player.currentHp, emeny.currentHp);
            }
            public void End(int playerHp, int emenyHp)
            {
               
                if (playerHp == 0 || emenyHp == 0)
                {
                    player.timerStamina.Dispose();
                    emenyTimer.Dispose();
                    pan.Disable();
                    Timer timer = new Timer() { Interval = 4000 };
                    timer.Tick += (object sender, EventArgs e) =>
                    {
                        timer.Dispose();
                        int reward = values["level"];
                        float coff;
                        if (difficultLevel == "困難") coff = 2;
                        else if (difficultLevel == "普通") coff = 1.5f;
                        else coff = 1;
                        form.Controls.Clear();
                        if (playerHp <= 0)
                        {
                            MessageBox.Show("敵人獲勝");
                        }
                        else
                        {
                            MessageBox.Show("玩家獲勝，獲得技能點數" + (int)(reward * coff) + "點");
                            values["skillPoint"] += (int)(reward * coff);
                        }
                        Prepare prepare = new Prepare(playerID, values, gameForm.getSubForm(), gameForm);

                        form.Dispose();
                    };
                    ProgressBar progressBar;
                    if (playerHp == 0)
                        progressBar = progressBarPlayer;
                    else
                        progressBar = progressBarEnemy;

                    Timer timer2 = new Timer() { Interval = 400 };
                    int startSign = 0, stopSign = 6;
                    timer2.Tick += (object sender2, EventArgs e2) =>
                    {
                        startSign++;
                        progressBar.Visible = !(progressBar.Visible);
                        if (startSign == stopSign)
                        {
                            progressBar.Visible = true;
                            timer2.Dispose();
                        }
                    };
                    timer.Start();
                    timer2.Start();


                }
            }
            public void Exit()
            {
                player.timerStamina.Dispose();
                emenyTimer.Dispose();
                
                Prepare prepare = new Prepare(playerID, values, gameForm.getSubForm(), gameForm);
                form.Dispose();
            }
        }
    }
}

