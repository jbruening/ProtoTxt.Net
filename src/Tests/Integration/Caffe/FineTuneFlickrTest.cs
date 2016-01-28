using System.IO;
using System.Linq;
using Caffe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtoTxt.Test.Integration.Caffe
{
    [TestClass]
    [DeploymentItem(@"Caffe\finetune_flickr_style\train_val.prototxt", "flickr")]
    [DeploymentItem(@"Caffe\finetune_flickr_style\deploy.prototxt", "flickr")]
    [DeploymentItem(@"Caffe\finetune_flickr_style\solver.prototxt", "flickr")]
    public class FineTuneFlickrTest
    {
        [TestMethod]
        public void TestDeployDeserialize()
        {
            var net = ProtoConvert.DeserializeObject<NetParameter>(File.ReadAllText(@"flickr\deploy.prototxt"));
            
            Assert.IsNotNull(net);
            Assert.IsNotNull(net.Input);
            Assert.AreEqual("data", net.Input[0]);
            Assert.AreEqual(10, net.InputShape[0].Dim[0]);
            Assert.AreEqual(1, net.Layer[0].Param[0].LrMult);
            Assert.AreEqual("gaussian", net.Layer[0].ConvolutionParam.WeightFiller.Type);
            Assert.AreEqual("prob", net.Layer.Last().Name);
        }

        [TestMethod]
        public void TestTrainValDeserialize()
        {
            var net = ProtoConvert.DeserializeObject<NetParameter>(File.ReadAllText(@"flickr\train_val.prototxt"));

            Assert.IsNotNull(net);
            Assert.AreEqual(26, net.Layer.Count);
            Assert.AreEqual("data/ilsvrc12/imagenet_mean.binaryproto", net.Layer[0].TransformParam.MeanFile);
            Assert.AreEqual(256u, net.Layer[1].ImageDataParam.NewHeight);
            Assert.AreEqual(Phase.TEST, net.Layer[24].Include[0].Phase);
            Assert.AreEqual("gaussian", net.Layer[23].InnerProductParam.WeightFiller.Type);
        }

        [TestMethod]
        public void TestSolverDeserialize()
        {
            var solve = ProtoConvert.DeserializeObject<SolverParameter>(File.ReadAllText(@"flickr\solver.prototxt"));

            Assert.IsNotNull(solve);
            Assert.AreEqual(20000, solve.Stepsize);
            Assert.AreEqual(20, solve.Display);
            Assert.AreEqual(0.0005f, solve.WeightDecay);
            Assert.AreEqual("models/finetune_flickr_style/finetune_flickr_style", solve.SnapshotPrefix);
        }
    }
}
