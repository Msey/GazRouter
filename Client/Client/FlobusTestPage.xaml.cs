using System;
using System.Windows;
using System.Windows.Controls;

namespace GazRouter.Client
{
    public partial class FlobusTestPage : UserControl
    {
        public FlobusTestPage()
        {
            InitializeComponent();
            Loaded += FlobusTestPage_Loaded;
        }

        private void FlobusTestPage_Loaded(object sender, RoutedEventArgs e)
        {
            var count = 0;
            for (var i = -100; i < 100; i++)
            {
                for (var j = -100; j < 100; j++)
                {
                    var endPoint = new Point(i*20 + 1000*(Math.Abs(j)%2), j*20 + 1000*((Math.Abs(j) + 1)%2));
//                    Schema.AddPipeline(new PipelineWidget(new PipelineSt));
                    count++;
                    if (count == 17000)
                    {
                        return;
                    }
                }
            }
            //  Schema.AddPipeline()
            //       return;
//            var pipeline = new PipelineWidget(Schema, new Point(100, 200), new Point(700, 400)) { StrokeThickness = 4, Stroke = Colors.Green, KmBegining = 0, KmEnd = 300};
//            pipeline.AddPipelinePoint(new PipelineP (100, new Point(300,200)));
//            pipeline.AddPipelinePoint( IntermediatePipelinePoint.CreateIntermediate(200, new Point(500, 400)));

//            Schema.AddPipeline(pipeline);

//            var valveWidget = new ValveWidget(pipeline, 150);
//            pipeline.AddPipelinePoint(valveWidget);
//            Schema.AddDiagramItem(valveWidget);
            /*            DiagramCompressorShop compressorShop = new DiagramCompressorShop(Schema) { Caption = "КЦ 1", Position = new Point(100,100)};
                        Schema.AddCompressorShop(compressorShop);
                        Schema.AddCompressorShop(new DiagramCompressorShop(Schema) { Caption = "КЦ 2", Position = new Point(300, 100),  Height = 100});
            */
        }
    }
}