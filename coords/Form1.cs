namespace coords;

public partial class Form1 : Form
{
    private readonly System.Windows.Forms.Timer _timer;

    public Form1()
    {
        InitializeComponent();

        _timer = new System.Windows.Forms.Timer
        {
            Interval = 100
        };
        _timer.Tick += (_, _) => UpdateCoords();
        _timer.Start();
    }

    private void UpdateCoords()
    {
        var pos = Cursor.Position;
        coordsLabel.Text = $"{pos.X}, {pos.Y}";
    }

    public void FormCloser(object sender, FormClosedEventArgs e)
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
