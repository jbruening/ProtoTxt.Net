using System.IO;
using System.Linq;
using Caffe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtoTxt.Test.Integration.Caffe
{
    [TestClass]
    [DeploymentItem(@"Caffe\bvlc_googlenet\train_val.prototxt", "bvlc_googlenet")]
    [DeploymentItem(@"Caffe\bvlc_googlenet\deploy.prototxt", "bvlc_googlenet")]
    public class GoogleNetTest
    {
        [TestMethod]
        public void TestDeployDeserialize()
        {
            var net = ProtoConvert.DeserializeObject<NetParameter>(File.ReadAllText(@"bvlc_googlenet\deploy.prototxt"));
            
            Assert.IsNotNull(net);
            Assert.IsNotNull(net.Input);
            Assert.AreEqual("data", net.Input[0]);
            Assert.AreEqual(10, net.InputShape[0].Dim[0]);
            Assert.AreEqual(2, net.Layer[0].Param[1].LrMult);
            Assert.AreEqual("xavier", net.Layer[0].ConvolutionParam.WeightFiller.Type);
            Assert.AreEqual("loss3/classifier", net.Layer.Last().Bottom[0]);
        }

        [TestMethod]
        public void TestTrainValDeserialize()
        {
            var net = ProtoConvert.DeserializeObject<NetParameter>(File.ReadAllText(@"bvlc_googlenet\train_val.prototxt"));

            Assert.IsNotNull(net);
            Assert.AreEqual(166, net.Layer.Count);
            Assert.AreEqual(117, net.Layer[0].TransformParam.MeanValue[1]);
            Assert.AreEqual(Phase.TEST, net.Layer[1].Include[0].Phase);
        }
    }
}
