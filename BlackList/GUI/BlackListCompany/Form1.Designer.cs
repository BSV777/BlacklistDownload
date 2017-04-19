namespace BlackListCompany
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btParse = new System.Windows.Forms.Button();
            this.btMakeXML = new System.Windows.Forms.Button();
            this.btSubscribe = new System.Windows.Forms.Button();
            this.btUpload = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tb1 = new System.Windows.Forms.TextBox();
            this.btDownload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btParse
            // 
            this.btParse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btParse.Location = new System.Drawing.Point(12, 128);
            this.btParse.Name = "btParse";
            this.btParse.Size = new System.Drawing.Size(360, 23);
            this.btParse.TabIndex = 0;
            this.btParse.Text = "5. Преобразовать dump.xml в rules.txt";
            this.btParse.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btParse.UseVisualStyleBackColor = true;
            this.btParse.Click += new System.EventHandler(this.btParse_Click);
            // 
            // btMakeXML
            // 
            this.btMakeXML.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btMakeXML.Location = new System.Drawing.Point(12, 12);
            this.btMakeXML.Name = "btMakeXML";
            this.btMakeXML.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btMakeXML.Size = new System.Drawing.Size(360, 23);
            this.btMakeXML.TabIndex = 1;
            this.btMakeXML.Text = "1. Создать Company.XML";
            this.btMakeXML.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btMakeXML.UseVisualStyleBackColor = true;
            this.btMakeXML.Click += new System.EventHandler(this.btMakeXML_Click);
            // 
            // btSubscribe
            // 
            this.btSubscribe.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btSubscribe.Location = new System.Drawing.Point(12, 41);
            this.btSubscribe.Name = "btSubscribe";
            this.btSubscribe.Size = new System.Drawing.Size(360, 23);
            this.btSubscribe.TabIndex = 2;
            this.btSubscribe.Text = "2. Подписать в OpenSSL";
            this.btSubscribe.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btSubscribe.UseVisualStyleBackColor = true;
            this.btSubscribe.Click += new System.EventHandler(this.btSubscribe_Click);
            // 
            // btUpload
            // 
            this.btUpload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btUpload.Location = new System.Drawing.Point(12, 70);
            this.btUpload.Name = "btUpload";
            this.btUpload.Size = new System.Drawing.Size(360, 23);
            this.btUpload.TabIndex = 4;
            this.btUpload.Text = "3. Отправить запрос в vigruzki.rkn.gov.ru";
            this.btUpload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btUpload.UseVisualStyleBackColor = true;
            this.btUpload.Click += new System.EventHandler(this.btUpload_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button1.Location = new System.Drawing.Point(12, 157);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(360, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "6. Загрузить правила в MikroTik";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // tb1
            // 
            this.tb1.Location = new System.Drawing.Point(12, 186);
            this.tb1.Multiline = true;
            this.tb1.Name = "tb1";
            this.tb1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb1.Size = new System.Drawing.Size(360, 564);
            this.tb1.TabIndex = 6;
            // 
            // btDownload
            // 
            this.btDownload.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btDownload.Location = new System.Drawing.Point(12, 99);
            this.btDownload.Name = "btDownload";
            this.btDownload.Size = new System.Drawing.Size(360, 23);
            this.btDownload.TabIndex = 7;
            this.btDownload.Text = "4. Проверить ответ от vigruzki.rkn.gov.ru";
            this.btDownload.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btDownload.UseVisualStyleBackColor = true;
            this.btDownload.Click += new System.EventHandler(this.btDownload_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 762);
            this.Controls.Add(this.btDownload);
            this.Controls.Add(this.tb1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btUpload);
            this.Controls.Add(this.btSubscribe);
            this.Controls.Add(this.btMakeXML);
            this.Controls.Add(this.btParse);
            this.MaximumSize = new System.Drawing.Size(400, 800);
            this.MinimumSize = new System.Drawing.Size(400, 800);
            this.Name = "Form1";
            this.Text = "Реестр запрещенных сайтов";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btParse;
        private System.Windows.Forms.Button btMakeXML;
        private System.Windows.Forms.Button btSubscribe;
        private System.Windows.Forms.Button btUpload;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox tb1;
        private System.Windows.Forms.Button btDownload;
    }
}

