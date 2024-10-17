using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mkey
{
    public class SlotGroupBehavior : MonoBehaviour
    {
        public Transform TilesGroup;
        public List<int> symbOrder;
        public RayCaster topRayCaster;
        public RayCaster middleRayCaster;
        public RayCaster bottomRayCaster;

        [Space(16, order = 0)]
        [SerializeField]
        [Tooltip("sec, additional rotation time")]
        private float addRotateTime = 0f;
        [SerializeField]
        [Tooltip("sec, delay time for spin")]
        private float spinStartDelay = 0f;
        [Tooltip("min 0% - max 20%, change spinStartDelay")]
        [SerializeField]
        private int spinStartRandomize = 0;
        [SerializeField]
        private int spinSpeedMultiplier = 1;

        private int tileCount;
        SlotSymbol[] slotSymbols;
        SlotIcon[] sprites;

        int nextReelSymbolToChange = 0;
        int nextReelSymbolToSet = 0;
        int currOrderPosition = 0;
        float anglePerTileRad = 0;
        float anglePerTileDeg = 0;
        float[] symbProbabilities;

        private void OnValidate()
        {
            spinStartRandomize = (int)Mathf.Clamp(spinStartRandomize, 0, 20);
            spinStartDelay = Mathf.Max(0,spinStartDelay);
            spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
            addRotateTime = Mathf.Max(0, addRotateTime);
        }

        public float[] SymbProbabilities
        {
            get { return symbProbabilities; }
        }

        /// <summary>
        /// Instantiate slot tiles 
        /// </summary>
        internal void CreateSlotCylinder(SlotIcon[] sprites, int tileCount, GameObject tilePrefab)
        {
            this.sprites = sprites;
            this.tileCount = tileCount;
            slotSymbols = new SlotSymbol[tileCount];
            float distTileY = 3.85f; //3.48f;

            float radius = ((tileCount + 1) * distTileY) / (2.0f * Mathf.PI);

            anglePerTileDeg = 360.0f / (float)tileCount;
            anglePerTileRad = anglePerTileDeg * Mathf.Deg2Rad;

            TilesGroup.localPosition = new Vector3(TilesGroup.localPosition.x, TilesGroup.localPosition.y, radius);
            for (int i = 0; i < tileCount; i++)
            {
                float tileAngleRad = i * anglePerTileRad;
                float tileAngleDeg = i * anglePerTileDeg;

                slotSymbols[i] = Instantiate(tilePrefab, transform.position, Quaternion.identity).GetComponent<SlotSymbol>();
                slotSymbols[i].transform.parent = TilesGroup;
                slotSymbols[i].transform.localPosition = new Vector3(0, radius * Mathf.Sin(tileAngleRad), -radius * Mathf.Cos(tileAngleRad));
                slotSymbols[i].transform.localScale = new Vector3(1f, 1f, 1f);
                slotSymbols[i].transform.localEulerAngles = new Vector3(tileAngleDeg, 0, 0);
                slotSymbols[i].name = "SlotSymbol: " + String.Format("{0:00}", i);
                slotSymbols[i].GetComponent<SpriteRenderer>().sortingOrder = SortingOrder.Symbols;
            }
            int length = symbOrder.Count;
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                int symNumber = symbOrder[GetNextSymb()];
                slotSymbols[i].SetIcon(sprites[symNumber], symNumber);
                slotSymbols[i].Position = i;
            }
            nextReelSymbolToSet = slotSymbols.Length;

            symbProbabilities = GetReelSymbHitPropabilities(sprites);
        }


        /// <summary>
        /// Async rotate cylinder
        /// </summary>
        internal void NextRotateCylinderEase(EaseAnim mainRotType, EaseAnim inRotType, EaseAnim outRotType,
                                        float mainRotTime, float mainRotateTimeRandomize,
                                        float inRotTime, float outRotTime,
                                        float inRotAngle, float outRotAngle,
                                        int nextOrderPosition,  Action rotCallBack)

        {
            addRotateTime = Mathf.Max(0, addRotateTime);
            mainRotateTimeRandomize = Mathf.Clamp(mainRotateTimeRandomize, 0f, 0.2f);
            mainRotTime = addRotateTime + UnityEngine.Random.Range(mainRotTime * (1.0f - mainRotateTimeRandomize), mainRotTime * (1.0f + mainRotateTimeRandomize));


            // start spin delay
            spinStartDelay = Mathf.Max(0, spinStartDelay);
            float spinStartRandomizeF = Mathf.Clamp(spinStartRandomize / 100f, 0f, 0.2f);
            float startDelay = UnityEngine.Random.Range(spinStartDelay * (1.0f - spinStartRandomizeF), spinStartDelay * (1.0f + spinStartRandomizeF));

            // check range before start
            inRotTime = Mathf.Clamp(inRotTime, 0, 1f);
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);

            outRotTime = Mathf.Clamp(outRotTime, 0, 1f);
            outRotAngle = Mathf.Clamp(outRotAngle, 0, 10);

            spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);

            // calc rotation angle to symbol in nextOrderPosition
            float angleX = GetAngleToNextSymb(nextOrderPosition) + anglePerTileDeg * symbOrder.Count * spinSpeedMultiplier;

            // set start rotation 
            float currAngleX = TilesGroup.localRotation.eulerAngles.x;
            currAngleX = Mathf.RoundToInt(currAngleX / anglePerTileDeg) * anglePerTileDeg;
            TilesGroup.localRotation = Quaternion.Euler(currAngleX, TilesGroup.localRotation.eulerAngles.y, TilesGroup.localRotation.eulerAngles.z);
            Vector3 startAngles = TilesGroup.localRotation.eulerAngles;//  Debug.Log("nexOrderPOsition: " + nextOrderPosition+ " ;nextSymbNumber: " + symbOrder[nextOrderPosition]+ " : "+ sprites[symbOrder[nextOrderPosition]].name + " ;angleX: " + angleX);

            // create reel rotation sequence - 3 parts  in - main - out
            float oldVal = 0f;
            TweenSeq tS = new TweenSeq();

            tS.Add((callBack) =>
            {
                // in rotation part
                SimpleTween.Value(TilesGroup.gameObject, 0f, inRotAngle, inRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (callBack != null) callBack();
                                  }).SetEase(inRotType).SetDelay(startDelay);
            });

            tS.Add((callBack) =>
            {
                // main rotation part
                SimpleTween.Value(TilesGroup.gameObject, inRotAngle, -(angleX + outRotAngle), mainRotTime) 
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                      ChangeIcon();
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      if (callBack != null) callBack();
                                  }).SetEase(mainRotType);
            });

            tS.Add((callBack) =>
            {
                // out rotation part
                SimpleTween.Value(TilesGroup.gameObject, -(angleX + outRotAngle), -angleX, outRotTime) 
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      currOrderPosition = nextOrderPosition;
                                      if (rotCallBack != null) rotCallBack();
                                      if (callBack != null) callBack();
                                  }).SetEase(outRotType);
            });

            tS.Start();
        }



        Vector3 pos;
        /// <summary>
        /// Change icon on reel appropriate to symbOrder
        /// </summary>
        private void ChangeIcon()
        {
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                pos = slotSymbols[i].transform.position - TilesGroup.position;

                int posOnReel = slotSymbols[i].Position;
                if (pos.y < 10 && pos.y > -10 && pos.z > 0 && posOnReel == nextReelSymbolToChange) // back side
                {
                    slotSymbols[i].Position = nextReelSymbolToSet;
                    int symNumber = symbOrder[GetNextSymb()];
                    slotSymbols[i].SetIcon(sprites[symNumber], symNumber);
                    nextReelSymbolToSet++;
                    nextReelSymbolToChange++;
                    return;
                }
            }
        }

        int next = 0;
        /// <summary>
        /// Return next symb position  in symbOrder array
        /// </summary>
        /// <returns></returns>
        private int GetNextSymb()
        {
            return (int)Mathf.Repeat(next++, symbOrder.Count);
        }

        /// <summary>
        /// Return angle in degree to next symbol position in symbOrder array
        /// </summary>
        /// <param name="nextOrderPosition"></param>
        /// <returns></returns>
        private float GetAngleToNextSymb(int nextOrderPosition)
        {
            if (currOrderPosition < nextOrderPosition)
            {
                return (nextOrderPosition - currOrderPosition) * anglePerTileDeg;
            }
            return (symbOrder.Count - currOrderPosition + nextOrderPosition) * anglePerTileDeg;
        }

        /// <summary>
        /// Return probabilties for eac symbol according to symbOrder array 
        /// </summary>
        /// <returns></returns>
        internal float[] GetReelSymbHitPropabilities(SlotIcon[] symSprites)
        {
            if (symSprites == null || symSprites.Length == 0) return null;
            float[] probs = new float[symSprites.Length];
            int length = symbOrder.Count;
            for (int i = 0; i < length; i++)
            {
                int n = symbOrder[i];
                probs[n]++;
            }
            for (int i = 0; i < probs.Length; i++)
            {
                probs[i] = probs[i] / (float)length;
            }
            return probs;
        }

        /// <summary>
        /// Return true if top, middle or bottom raycaster has symbol with ID == symbID
        /// </summary>
        /// <param name="symbID"></param>
        /// <returns></returns>
        public bool HasSymbolInAnyRayCaster(int symbID, ref List<SlotSymbol> slotSymbols)
        {
            slotSymbols = new List<SlotSymbol>();
            bool res = false;

            SlotSymbol tS = topRayCaster.GetSymbol();
            if (tS.iconID == symbID)
            {
                res = true;
                slotSymbols.Add(tS);
            }

            tS = middleRayCaster.GetSymbol();
            if (tS.iconID == symbID)
            {
                res = true;
                slotSymbols.Add(tS);
            }

            tS = bottomRayCaster.GetSymbol();
            if (tS.iconID == symbID)
            {
                res = true;
                slotSymbols.Add(tS);
            }
            return res;
        }
    }
}
