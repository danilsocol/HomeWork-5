using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HomeWork_5;
using HomeWork5.Logic;

namespace HomeWork5
{
    public partial class MainWindow : Window
    {
        bool create = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        int id = 1;

        bool FirstCoord = true;
        double y1;
        double x1;
        string nameFirstNode;
        bool createEdge = false;
        Graph graph = new Graph();
        int countNode=0;
        public void ClickNode(object sender, RoutedEventArgs e)
        {
            if (!create)
            {
                DeleteNode(sender, e);
                
            }
            else
            {
                CreateEdge(sender, e);
                createEdge = true;
                
            }
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (create && !createEdge)
            {

                int newId = FindId();
                
                var point = Mouse.GetPosition(this);

                CreateNode( point.X, point.Y, newId);

                graph.AddNode(newId);

                id++;
            }
            createEdge = false;
        }

        void CreateNode(double x, double y,int id)
        {
            TextBlock tb = new TextBlock();
            Ellipse el = new Ellipse();

            el.Width = 30;
            el.Height = 30;
            el.VerticalAlignment = VerticalAlignment.Top;
            el.Margin = new Thickness(x - 15, y - 110, 0, 0);
            el.Fill = Brushes.Gray;
            el.Name = $"id{id}";

            tb.Name = $"tb_id{id}";
            tb.Text = $"{id}";
            tb.Foreground = Brushes.DarkRed;
            tb.FontSize = 18;
            tb.Margin = new Thickness(x - 30, y - 130, 0, 0);

            el.MouseDown += new MouseButtonEventHandler(ClickNode);

            Canvas.SetZIndex(tb, 2);
            Canvas.SetZIndex(el, 1);
            canvas.Children.Add(el);
            canvas.Children.Add(tb);
            countNode++;
        }

        int FindId()
        {
            for (int i = 1; i <= id+1; i++)
            {
                bool ThereIsANode = false;
                var images = canvas.Children.OfType<Ellipse>().ToList();
                foreach (var image in images)
                {
                    if (image.Name == $"id{i}")
                    {
                        ThereIsANode = true;
                    }
                }
                if (!ThereIsANode)
                {
                    return i;
                }
            }

            return 0;
        }
        void DeleteNode(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(sender as Ellipse);
            graph.DeleteNode(Convert.ToInt32((sender as Ellipse).Name.Remove(0, 2)));

            deleteNode = true;
            DeleteNodeEdge(sender, e);
            DeleteNodeTB(sender, e);
            deleteNode = false;
            countNode--;
        }

        void DeleteNodeTB(object sender, RoutedEventArgs e)
        {
            var images = canvas.Children.OfType<TextBlock>().ToList();
            foreach (var image in images)
            {
                if (image.Name == $"tb_{(sender as Ellipse).Name}")
                {
                    canvas.Children.Remove(image);
                }
            }
        }

        void DeleteNodeEdge(object sender, RoutedEventArgs e)
        {
            var images = canvas.Children.OfType<Line>().ToList();
            foreach (var image in images)
            {
                var name = image.Name.Split("_");
                for (int i = 0; i < 2; i++)
                {
                    if (name[i] == $"{(sender as Ellipse).Name}")
                    {
                        ClickEdge(image, e);
                        //canvas.Children.Remove(sender as Line);
                    }

                }

            }
        }

        void SpecifySecondNode(double x2, double y2, string name, int price ,bool download = false)
        {
            Line line = new Line();
            TextBox textBox = new TextBox();
            TextBlock textBlock = new TextBlock();
            line.Y1 = y1 - 15;
            line.X1 = x1 - 15;
            line.Y2 = y2 - 15;
            line.X2 = x2 - 15;
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;

            line.MouseEnter += new MouseEventHandler(LineEnter);
            line.MouseLeave += new MouseEventHandler(LineLeave);
            line.MouseDown += new MouseButtonEventHandler(ClickEdge);
            if (!download)
                line.Name = nameFirstNode + $"_{name}";
            else 
                line.Name = $"{name}";

            textBox.Text = $"{price}";
            textBox.Foreground = Brushes.DarkRed;
            textBox.FontSize = 16;
            textBox.Margin = new Thickness((line.X1 + line.X2) / 2 - 15, (line.Y1 + line.Y2) / 2 - 30, 0, 0);
            textBox.Name = $"meaning_{line.Name}";
            textBox.PreviewKeyDown += new KeyEventHandler(TextChanged);

            if(!download)
                textBlock.Text = $"->{name.Remove(0,2)}";
            else
                textBlock.Text = $"->{name.Split("_")[1].Remove(0, 2)}";

            textBlock.Margin = new Thickness((line.X1 + line.X2) / 2 - 15, (line.Y1 + line.Y2) / 2 - 50, 0, 0);
            textBlock.Name = $"direction_{line.Name}";
            textBlock.FontSize = 16;

            Canvas.SetZIndex(textBox, 2);
            Canvas.SetZIndex(textBlock, 2);
            Canvas.SetZIndex(line, 0);

            canvas.Children.Add(line);
            canvas.Children.Add(textBox);
            canvas.Children.Add(textBlock);

            FirstCoord = true;
        }

        Node firstNode;
        void CreateEdge(object sender, RoutedEventArgs e)
        {
            if (FirstCoord)
            {
                y1 = (sender as Ellipse).DesiredSize.Height;
                x1 = (sender as Ellipse).DesiredSize.Width;
                nameFirstNode = $"{(sender as Ellipse).Name}";

                FirstCoord = false;

                firstNode = graph.FindNode(Convert.ToInt32((sender as Ellipse).Name.Remove(0,2)));
            }
            else
            {
                SpecifySecondNode((sender as Ellipse).DesiredSize.Width, (sender as Ellipse).DesiredSize.Height,(sender as Ellipse).Name, 00);

                Node secondNode = graph.FindNode(Convert.ToInt32((sender as Ellipse).Name.Remove(0, 2)));
                graph.AddEdge(firstNode, secondNode, 0, nameFirstNode + $"_{(sender as Ellipse).Name}");

            }
        }

        void TextChanged(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string[] id = FindIdNodeTextBox(sender);
                graph.ChangePrice(Convert.ToInt32(id[0]), Convert.ToInt32(id[1]), Convert.ToInt32((sender as TextBox).Text));
            }
        }

        string[] FindIdNodeTextBox(object sender)
        {
           string[] id = (sender as TextBox).Name.Remove(0, 8).Split("_");
            id[0] = id[0].Trim(new char[] { 'i', 'd' });
            id[1] = id[1].Trim(new char[] { 'i', 'd' });
            return id;
        }

        string[] FindIdNodeLine(object sender)
        {
            string[] id;

            if ((sender as Line) != null)
                id = (sender as Line).Name.Split("_");
            else
                id  = (sender as string).Split("_");

            id[0] = id[0].Trim(new char[] { 'i', 'd' });
            id[1] = id[1].Trim(new char[] { 'i', 'd' });
            return id;
        }

        void LineEnter(object sender, MouseEventArgs e)
        {
            (sender as Line).Stroke = Brushes.OrangeRed;
        }
        void LineLeave(object sender, MouseEventArgs e)
        {
            (sender as Line).Stroke = Brushes.Black;
        }

        bool deleteNode;
        void ClickEdge(object sender, RoutedEventArgs e)
        {
            if (!create)
            {
                canvas.Children.Remove(sender as Line);
                string[] id = FindIdNodeLine(sender);
                if(!deleteNode)
                    graph.DeleteEdge(Convert.ToInt32(id[0]), Convert.ToInt32(id[1]));
                DeleteEdgeMeaning(sender, e);
                DeleteEdgeDirection(sender, e);
                
            }
        }
        void DeleteEdgeMeaning(object sender, RoutedEventArgs e)
        {
            var images = canvas.Children.OfType<TextBox>().ToList();
            foreach (var image in images)
            {
                var name = image.Name;

                if (name == $"meaning_{(sender as Line).Name}")
                    canvas.Children.Remove(image);
            }
        }

        void DeleteEdgeDirection(object sender, RoutedEventArgs e)
        {
            var images = canvas.Children.OfType<TextBlock>().ToList();
            foreach (var image in images)
            {
                var name = image.Name;

                if (name == $"direction_{(sender as Line).Name}")
                    canvas.Children.Remove(image);
            }
        }
        

        private void btn_CreateNode_Click(object sender, RoutedEventArgs e)
        {
            if (create)
            {
                create = false;
                this.btn_CreateNode.Content = "Удалить";
            }
            else
            {
                create = true;
                this.btn_CreateNode.Content = "Создать";
            }

        }

        void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            File.Delete("saveLogic.txt");
            File.Delete("save.txt");

            graph.Save("saveLogic.txt");
            List<Ellipse> listNode = new List<Ellipse>();
            List<Line> listEdge = new List<Line>();
            SplitCanvas(sender, listNode, listEdge);
            using (FileStream save = new FileStream($"save.txt", FileMode.OpenOrCreate))
            {
                int i = 0;
                while (i< listNode.Count+ listEdge.Count)
                {
                    if (i<listNode.Count)
                    {
                        string nodes = $"{listNode[i].Name} {listNode[i].Margin.Left+15} {listNode[i].Margin.Top+110}|";
                        byte[] array = System.Text.Encoding.Default.GetBytes(nodes);
                        save.Write(array, 0, array.Length);
                        save.Seek(1, SeekOrigin.End);
                        i++;
                    }
                    else 
                    {
                        string edges = $"{listEdge[i - listNode.Count].Name} {listEdge[i - listNode.Count].X1+15} {listEdge[i - listNode.Count].Y1+15}" +
                            $" {listEdge[i - listNode.Count].X2+15} {listEdge[i - listNode.Count].Y2+15} {FindPrice(listEdge[i - listNode.Count].Name)}|";
                        byte[] array = System.Text.Encoding.Default.GetBytes(edges);
                        save.Write(array, 0, array.Length);
                        save.Seek(1, SeekOrigin.End);
                        i++;
                    }
                }

            }
        }
        void SplitCanvas(object sender, List<Ellipse> listNode, List<Line> listEdge)
        {
            var images = canvas.Children.OfType<Shape>().ToList();
            foreach (object image in images)
            {
                if((image as Ellipse) != null)
                {
                    listNode.Add((Ellipse)image);
                }
                else if((image as Line) != null)
                {
                    listEdge.Add((Line)image);
                }

            }
        }

        int FindPrice(string nameEdge)
        {
            var images = canvas.Children.OfType<TextBox>().ToList();
            foreach (object image in images)
            {
                if ((image as TextBox).Name.Remove(0,8) == nameEdge)
                {
                    return Convert.ToInt32((image as TextBox).Text);
                }
            }
            return 0;
        }


        void btn_Download_Click(object sender, RoutedEventArgs e)
        {
            id = 0;
            canvas.Children.Clear();
            graph.Download("saveLogic.txt");

            using (FileStream save = new FileStream($"save.txt", FileMode.OpenOrCreate))
            {
                save.Seek(0, SeekOrigin.Begin);
                byte[] output = new byte[save.Length];
                save.Read(output, 0, output.Length);

                string[] textGraph = Encoding.Default.GetString(output).Split("|");

                for (int i = 0; i < textGraph.Length-1; i++)
                {
                    string[] split = textGraph[i].Split(" ");

                    if (i != 0)
                    {
                        split[0] = split[0].Remove(0, 2);
                    }

                    if (textGraph[i].Split("_").Length == 1)
                    {
                        CreateNode( Convert.ToDouble(split[1]), Convert.ToDouble(split[2]), Convert.ToInt32(split[0].Trim(new char[] { 'i', 'd' })));

                        graph.AddNode(Convert.ToInt32(split[0].Trim(new char[] { 'i', 'd' })));

                        id++;
                    }
                    else
                    {
                        y1 = Convert.ToDouble(split[2]);
                        x1 = Convert.ToDouble(split[1]);
                        nameFirstNode = $"";

                        string[] namesNode = FindIdNodeLine(split[0]);

                        Node firstNode = graph.FindNode(Convert.ToInt32(namesNode[0]));
                        Node secondNode = graph.FindNode(Convert.ToInt32(namesNode[1]));

                        if (split[0][0] != 'd' && split[0][0] != 'i')
                            split[0] = $"d{split[0]}";

                        if (split[0][0] != 'i')
                            split[0] = $"i{split[0]}";

                        SpecifySecondNode( Convert.ToDouble(split[3]), Convert.ToDouble(split[4]), split[0], Convert.ToInt32(split[5]), true);

                        graph.AddEdge(firstNode, secondNode, Convert.ToInt32(split[5]), split[0]);
                    }

                }
            }
            
        }

        void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            graph = new Graph();
            FirstCoord = true;
        }

        async void btn_StartMaxThroughput_Click(object sender, RoutedEventArgs e)
        {
            int[,] table = graph.transformationGraph();

            int[,] rGraph = new int[(int)Math.Sqrt(table.Length), (int)Math.Sqrt(table.Length)];

            int idStart = Convert.ToInt32(tb_FromNode.Text) - 1;
            int idFinal = Convert.ToInt32(tb_BeforeNode.Text) - 1;

            Array.Copy(table, rGraph, table.Length);

            int count = 0;

            int[] parent = new int[countNode];

            int max_flow = 0;
            List<int> route = new List<int>();

            while (MaxFlow.bfs(rGraph, idStart, idFinal, parent, countNode))
            {
                count++;
                int min = MaxFlow.fordFulkerson(table, rGraph, idStart,  idFinal, parent, ref route);

                List<Ellipse> listEl = FindNodeById(route);
                List<Line> listLine = FindEdgeById(route);
                ShowAction(min, route, count, listEl, listLine);
                await Task.Delay(3000);

                BackColorNodeAndEdge(route, listEl, listLine);

                max_flow += min;
            }

            tb_Return.Text = $"Ответ: {max_flow}";
        }
        void ShowAction(int min, List<int> route, int count, List<Ellipse> listEl, List<Line> listLine)
        {
            string str = "";
            for (int i = 0; i < route.Count; i++)
            {
                str += $"{route[route.Count-i-1]+1} ";
            }
            PaintNodeAndEdge(route, listEl, listLine);
            tb_logs.Text += $"{count}) Идем по вершинам с id {str} затем выбераем ребро с самым минимальным пропусным значением {min}\n";

        }

        void PaintNodeAndEdge(List<int> route, List<Ellipse> listEl, List<Line> listLine) //Brushes.Gray
        {
            for (int i = 0; i < route.Count; i++)
            {
                listEl[i].Fill = Brushes.DarkRed;
                if (i != 0)
                    listLine[i - 1].Stroke = Brushes.DarkRed;
            }
        }

        void BackColorNodeAndEdge(List<int> route, List<Ellipse> listEl, List<Line> listLine)
        {
            for (int i = 0; i < route.Count; i++)
            {
                listEl[i].Fill = Brushes.Gray;
                if (i != 0)
                    listLine[i - 1].Stroke = Brushes.Black;
            }
        }
       List<Ellipse> FindNodeById(List<int> route)
        {
            List<Ellipse> el = new List<Ellipse>();
            var images = canvas.Children.OfType<Ellipse>().ToList();
            foreach (var image in images)
            {
                for (int i = 0; i < route.Count; i++)
                {
                    if (image.Name == $"id{route[i]+1}")
                        el.Add(image);
                }
                
            }
            return el;
        }

        List<Line> FindEdgeById(List<int> route)
        {
            List<Line> line = new List<Line>();
            var images = canvas.Children.OfType<Line>().ToList();
            foreach (var image in images)
            {
                for (int i = 0; i < route.Count-1; i++) //route[route.Count - 1-i] _ route[route.Count - 2-i]
                {
                    if (image.Name == $"id{route[route.Count - 1 - i]+1}_id{route[route.Count - 2 - i]+1}")
                        line.Add(image);
                }

            }
            return line;
        }
    }
}