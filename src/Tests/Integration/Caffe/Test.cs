using System.IO;
using System.Linq;
using Caffe;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProtoTxt.Test.Integration.Caffe
{
    [TestClass]
    public class Test
    {
        [TestMethod]
        [DeploymentItem("deploy.prototxt")]
        public void TestDeployDeserialize()
        {
            var net = ProtoConvert.DeserializeObject<NetParameter>(File.ReadAllText(@"deploy.prototxt"));
            
            Assert.IsNotNull(net);
            Assert.IsNotNull(net.Input);
            Assert.AreEqual("data", net.Input[0]);
            Assert.AreEqual(10, net.InputShape[0].Dim[0]);
            Assert.AreEqual(2, net.Layer[0].Param[1].LrMult);
            Assert.AreEqual("xavier", net.Layer[0].ConvolutionParam.WeightFiller.Type);
            Assert.AreEqual("loss3/classifier", net.Layer.Last().Bottom[0]);
        }
    }
}
