using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using Gtk;

public partial class MainWindow : Gtk.Window
{
    string ConnectionString;
    List<string> RemovedElement = new List<string>();


    public MainWindow() : base(Gtk.WindowType.Toplevel)
    {
        Build();
        //
        // Color changes
        //
        ModifyBg(StateType.Normal, new Gdk.Color(38, 31, 31));
        label1.ModifyFg(StateType.Normal, new Gdk.Color(237, 227, 227));
        DatabaseEntry.ModifyBase(StateType.Normal, new Gdk.Color(89, 82, 82));
        DatabaseEntry.ModifyText(StateType.Normal, new Gdk.Color(237, 227, 227));
        label2.ModifyFg(StateType.Normal, new Gdk.Color(237, 227, 227));
        UserNameEntry.ModifyBase(StateType.Normal, new Gdk.Color(89, 82, 82));
        UserNameEntry.ModifyText(StateType.Normal, new Gdk.Color(237, 227, 227));
        label3.ModifyFg(StateType.Normal, new Gdk.Color(237, 227, 227));
        PasswordEntry.ModifyBase(StateType.Normal, new Gdk.Color(89, 82, 82));
        PasswordEntry.ModifyText(StateType.Normal, new Gdk.Color(237, 227, 227));
        label4.ModifyFg(StateType.Normal, new Gdk.Color(237, 227, 227));
        combobox1.ModifyBase(StateType.Normal, new Gdk.Color(89, 82, 82));
        label5.ModifyFg(StateType.Normal, new Gdk.Color(237, 227, 227));
        SearchEntry.ModifyBase(StateType.Normal, new Gdk.Color(155, 149, 149));
    }

    protected void OnDeleteEvent(object sender, DeleteEventArgs a)
    {
        Application.Quit();
        a.RetVal = true;
    }

    protected void OnHidden(object sender, EventArgs e)
    {
        Environment.Exit(1);
    }

    protected void OnCloseButtonPressed(object sender, EventArgs e)
    {
        Hide();
    }

    protected void OnTestConnectionButtonPressed(object sender, EventArgs e)
    {
        Gtk.TreeIter _iter;

        ConnectionString = "Server=" + DatabaseEntry.Text + ";" +
                           "User ID=" + UserNameEntry.Text + ";" +
                           "Password=" + PasswordEntry.Text;

        using (SqlConnection _conn = new SqlConnection(ConnectionString))
        {
            try
            {
                _conn.Open();
                MessageDialog _MessageDialog = new MessageDialog(this,
                                                                 DialogFlags.Modal,
                                                                 MessageType.Info,
                                                                 ButtonsType.Close,
                                                                 "Successfully connected to SQL Server");

                _MessageDialog.Title = "Connected to Database";
                _MessageDialog.Run();
                _MessageDialog.Destroy();
                using (SqlCommand _cmd = new SqlCommand("SELECT name FROM SYS.DATABASES", _conn))
                {
                    using (SqlDataReader _reader = _cmd.ExecuteReader())
                    {
                        //
                        // Clear ComboBox values
                        //
                        ListStore _ClearCombo = new ListStore(typeof(string), typeof(string));
                        combobox1.Model = _ClearCombo;
                        //
                        // Add new values to ComboBox
                        //
                        while (_reader.Read())
                        {
                            combobox1.AppendText(_reader["name"].ToString());
                        }
                    }
                }
                //
                // Select 1st database from results
                //
                combobox1.Model.IterNthChild(out _iter, 0);
                combobox1.SetActiveIter(_iter);
            }
            catch (Exception ex)
            {
                MessageDialog _MessageDialog = new MessageDialog(this,
                                                                 DialogFlags.Modal,
                                                                 MessageType.Error,
                                                                 ButtonsType.Close,
                                                                 "Connection failed to SQL Server\n"
                                                                    + ex.Message);
                _MessageDialog.Title = "Database connection failed";
                _MessageDialog.Run();
                _MessageDialog.Destroy();
            }
        }
    }

    protected void OnCombobox1Changed(object sender, EventArgs e)
    {
        CheckButton _btn;

        ConnectionString += ";Database=" + combobox1.ActiveText;

        foreach (var item in TableBox.Children)
        {
            TableBox.Remove(item);
        }

        using (SqlConnection _conn = new SqlConnection(ConnectionString))
        {
            _conn.Open();

            using (SqlCommand _cmd = new SqlCommand(
                @"SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' ORDER BY TABLE_NAME DESC",
                _conn))
            {
                using (SqlDataReader _reader = _cmd.ExecuteReader())
                {
                    while (_reader.Read())
                    {
                        _btn = new CheckButton();
                        _btn.Label = _reader["TABLE_NAME"].ToString();
                        _btn.Visible = true;
                        _btn.Active = false;
                        TableBox.PackEnd(_btn);
                    }
                }
            }
        }
    }

    protected void OnCheckAllButtonPressed(object sender, EventArgs e)
    {
        if (TableBox.Children.Length > 0)
        {
            foreach (CheckButton item in TableBox.Children)
            {
                item.Active = true;
            }
        }
    }

    protected void OnUnCheckAllButtonPressed(object sender, EventArgs e)
    {
        if (TableBox.Children.Length > 0)
        {
            foreach (CheckButton item in TableBox.Children)
            {
                item.Active = false;
            }
        }
    }

    protected void OnGenerateButtonPressed(object sender, EventArgs e)
    {
        string _path;
        foreach (CheckButton item in TableBox.Children)
        {
            if (item.Active)
            {
                using (SqlConnection _conn = new SqlConnection(ConnectionString))
                {
                    _conn.Open();
                    using (SqlCommand _cmd = new SqlCommand(
                    "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + item.Label + "'",
                        _conn))
                    {
                        using (SqlDataReader _reader = _cmd.ExecuteReader())
                        {
                            try
                            {
                                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/Classes");
                                _path = Directory.GetCurrentDirectory() + "/Classes/" + item.Label + "s.cs";
                                using (TextWriter _File = new StreamWriter(_path))
                                {
                                    _File.WriteLine("using System.ComponentModel.DataAnnotations.Schema;");
                                    _File.WriteLine("\n[Table(\"" + item.Label + "\")]");
                                    _File.WriteLine("public class " + item.Label + "s");
                                    _File.WriteLine("{");

                                    while (_reader.Read())
                                    {
                                        _File.WriteLine("\tpublic " +
                                                        EntitiesDictionary[_reader["DATA_TYPE"].ToString()]
                                                        + " " + _reader["COLUMN_NAME"] +
                                                        " { get; set; }\n");
                                    }

                                    _File.WriteLine("}");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageDialog _ErrorMessageDialog = new MessageDialog(this,
                                                                 DialogFlags.Modal,
                                                                 MessageType.Error,
                                                                 ButtonsType.Close,
                                                                 ex.Message);
                                _ErrorMessageDialog.Title = "File Generation Failed";
                                _ErrorMessageDialog.Run();
                                _ErrorMessageDialog.Destroy();
                            }
                        }
                    }
                }

                MessageDialog _MessageDialog = new MessageDialog(this,
                                                                 DialogFlags.Modal,
                                                                 MessageType.Info,
                                                                 ButtonsType.Close,
                                                                 "File Generation Finished");
                _MessageDialog.Title = "File Generated";
                _MessageDialog.Run();
                _MessageDialog.Destroy();
            }
        }
    }

    [GLib.ConnectBefore]
    protected void OnDatabaseEntryKeyPressEvent(object o, KeyPressEventArgs args)
    {
        if (args.Event.Key == Gdk.Key.Return)
        {
            TestConnectionButton.Press();
        }
    }

    [GLib.ConnectBefore]
    protected void OnUserNameEntryKeyPressEvent(object o, KeyPressEventArgs args)
    {
        if (args.Event.Key == Gdk.Key.Return)
        {
            TestConnectionButton.Press();
        }
    }

    [GLib.ConnectBefore]
    protected void OnPasswordEntryKeyPressEvent(object o, KeyPressEventArgs args)
    {
        if (args.Event.Key == Gdk.Key.Return)
        {
            TestConnectionButton.Press();
        }
    }

    protected void OnSearchEntryChanged(object sender, EventArgs e)
    {
        foreach (CheckButton item in TableBox.Children)
        {
            if (!item.Label.ToUpper().Contains(SearchEntry.Text.ToUpper()))
            {
                item.Visible = false;
            }
            else
            {
                item.Visible = true;
            }
        }

        //_threads = TableBox.Children.Length / (decimal)10.0;
        //_threadsRounded = (int)Math.Round(_threads, MidpointRounding.AwayFromZero);

        //_counter = 0;

        //for (int i = 1; i < 10; i++)
        //for (int j = _counter; j < Math.Truncate(_threads) * i; j++)
        //{
        //    ...
        //    _counter = (int)Math.Truncate(_threads) * i;
        //}
    }
}
