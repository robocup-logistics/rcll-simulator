using System.Collections.Generic;
using LlsfMsgs;
using Newtonsoft.Json;
using static LlsfMsgs.Order.Types;

namespace Simulator.Utility {
    [JsonObject(MemberSerialization.OptIn)]
    public class Products {
        private int WorkpieceRedRangeStart = 1000;
        private int WorkpieceBlackRangeStart = 2000;
        private int WorkpieceSilverRangeStart = 3000;
        private int WorkpieceColorlessRangeStart = 4000;
        private static int internalProductCounter;
        [JsonProperty]
        public int ID { get; private set; }
        [JsonProperty]
        public Complexity Complexity { get; private set; }
        [JsonProperty]
        public int RingCount { get; private set; }
        [JsonProperty]
        public BaseElement? Base { get; private set; }
        [JsonProperty]
        public CapElement? Cap { get; private set; }
        [JsonProperty]
        public List<RingElement> RingList { get; private set; }
        public void AddPart(BaseElement newBase) {
            Base = newBase;
        }
        public void AddPart(CapElement newCap) {
            Cap = newCap;
            Complexity++;
        }
        public void AddPart(RingElement newRing) {
            RingList.Add(newRing);
            Complexity++;
            RingCount++;
        }
        public string ProductDescription() {
            var capString = Cap != null ? Cap.GetText() : "NoCap";
            var baseString = Base != null ? Base.GetText() : "NoBase";
            var ringString = "";
            foreach (var r in RingList) {
                ringString += r.GetText() + " - ";
            }
            return baseString + " - " + ringString + capString;
        }
        public Products(BaseColor color) {
            Base = new BaseElement(color);
            Complexity = (Complexity) - 1;
            RingCount = 0;
            RingList = new List<RingElement>();
            switch (color) {
                case BaseColor.BaseBlack:
                    ID = WorkpieceBlackRangeStart;
                    break;
                case BaseColor.BaseRed:
                    ID = WorkpieceRedRangeStart;
                    break;
                case BaseColor.BaseSilver:
                    ID = WorkpieceSilverRangeStart;
                    break;
                case BaseColor.BaseClear:
                    ID = WorkpieceColorlessRangeStart;
                    break;
            }
            //FIXME RACY
            ID += internalProductCounter;
            internalProductCounter++;
        }
        public Products(CapColor color) {
            Base = new BaseElement();
            Cap = new CapElement(color);
            Complexity = (Complexity)0;
            RingCount = 0;
            RingList = new List<RingElement>();
            ID = WorkpieceColorlessRangeStart + internalProductCounter;
            internalProductCounter++;
        }

        public Products(RingColor color) {
            RingList = new List<RingElement>();
            RingList.Add(new RingElement(color));
            Complexity = (Complexity)1;
            RingCount = 0;
            RingList = new List<RingElement>();
        }
        public Products() {
            RingList = new List<RingElement>();
            Complexity = 0;
        }
        public CapElement? RetrieveCap() {
            if (Cap != null) {
                var cap = Cap;
                Cap = null;
                return cap;
            }
            return null;
        }

        public LlsfMsgs.WorkpieceDescription GetProtoDescription() {
            var desc = new LlsfMsgs.WorkpieceDescription {};

            if(Base != null) {
                desc.BaseColor = Base.BaseColor;
            }

            foreach(var ring in RingList) {
                desc.RingColors.Add(ring.RingColor);
            }

            if(Cap != null) {
                desc.CapColor = Cap.CapColor;
            }

            return desc;
        }
    }

    public class BaseElement// : Products
    {
        [JsonProperty]
        public BaseColor BaseColor { get; private set; }
        public BaseElement(BaseColor color) {
            BaseColor = color;
        }
        public BaseElement() {
            BaseColor = BaseColor.BaseClear;
        }
        public BaseColor GetBaseColor() {
            return BaseColor;
        }

        public string GetText() {
            return BaseColor.ToString();
        }
    }
    public class RingElement// : Products
    {
        [JsonProperty]
        public RingColor RingColor { get; private set; }
        public RingElement(RingColor color) {
            RingColor = color;
        }
        public RingColor GetRingColor() {
            return RingColor;
        }
        public string GetText() {
            return RingColor.ToString();
        }
    }
    public class CapElement// : Products
    {
        [JsonProperty]
        public CapColor CapColor { get; private set; }
        public CapElement(CapColor color) {
            CapColor = color;
        }
        public CapColor GetCapColor() {
            return CapColor;
        }
        public string GetText() {
            return CapColor.ToString();
        }
    }
}
