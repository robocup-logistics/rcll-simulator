﻿using System.Collections.Generic;
using LlsfMsgs;
using static LlsfMsgs.Order.Types;

namespace Simulator.Utility
{
    public class Products
    {
        public int ID { get; private set; }
        private Complexity Complexity;
        public BaseElement? Base;
        private CapElement? Cap;
        private List<RingElement> RingList = new List<RingElement>();
        public void AddPart(BaseElement newBase)
        {
            Base = newBase;
            Complexity++;
        }
        public void AddPart(CapElement newCap)
        {
            Cap = newCap;
            Complexity++;
        }
        public void AddPart(RingElement newRing)
        {
            RingList.Add(newRing);
            Complexity++;
        }
        public string ProductDescription()
        {
            var capString = Cap != null ? Cap.GetText() : "NoCap";
            var baseString = Base != null ? Base.GetText() : "NoBase";
            var ringString = "";
            foreach (var r in RingList)
            {
                ringString += r.GetText() + "\n";
            }
            return capString + "\n" + ringString + baseString;
        }
        public Products(BaseColor color)
        {
            Base =  new BaseElement(color);
        }
        public Products(CapColor color)
        {
            Base = new BaseElement();
            Cap = new CapElement(color);

        }

        public Products(RingColor color)
        {
            RingList.Add(new RingElement(color));
        }
        public Products()
        {
            
        }
        public CapElement? RetrieveCap()
        {
            if(Cap != null)
            {
                var cap = Cap;
                Cap = null;
                return cap;
            }
            return null;
        }
    }

    public class BaseElement : Products
    {
        private readonly BaseColor BaseColor;
        public BaseElement(BaseColor color)
        {
            BaseColor = color;
        }
        public BaseElement()
        {
            BaseColor = 0;
        }
        public BaseColor GetBaseColor()
        {
            return BaseColor;
        }

        public string GetText()
        {
            return BaseColor.ToString();
        }
    }
    public class RingElement : Products
    {
        private readonly RingColor RingColor;
        public RingElement(RingColor color)
        {
            RingColor = color;
        }
        public RingColor GetRingColor()
        {
            return RingColor;
        }
        public string GetText()
        {
            return RingColor.ToString();
        }
    }
    public class CapElement : Products
    {
        private readonly CapColor CapColor;
        public CapElement(CapColor color)
        {
            CapColor = color;
        }
        public CapColor GetRingColor()
        {
            return CapColor;
        }
        public string GetText()
        {
            return CapColor.ToString();
        }
    }
}