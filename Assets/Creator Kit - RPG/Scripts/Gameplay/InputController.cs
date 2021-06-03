using RPGM.Core;
using RPGM.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using TMPro;

namespace RPGM.UI
{
    /// <summary>
    /// Sends user input to the correct control systems.
    /// </summary>
    /// 
    public class InputController : MonoBehaviour
    {
        public TextMeshPro myTMP;
        private KeywordRecognizer reconocePalabras;
        private ConfidenceLevel confianza = ConfidenceLevel.Low;
        private Dictionary<string, Accion> palabrasAccion = new Dictionary<string, Accion>();
        
        public float stepSize = 0.1f;
        GameModel model = Schedule.GetModel<GameModel>();


        private delegate void Accion();
        private string keyword;

         void Start()
        {

            palabrasAccion.Add("up",null);
            palabrasAccion.Add("down",null);
            palabrasAccion.Add("left",null);
            palabrasAccion.Add("right",null);
            palabrasAccion.Add("stop",null);
            palabrasAccion.Add("yes",null);

            reconocePalabras = new KeywordRecognizer(palabrasAccion.Keys.ToArray(),confianza);
            reconocePalabras.OnPhraseRecognized += OnKeywordsRecognized;
            reconocePalabras.Start();
         }
         void OnDestroy()
        {
            if(reconocePalabras != null && reconocePalabras.IsRunning)
            {
                reconocePalabras.Stop();
                reconocePalabras.Dispose();
            }
        }
         private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
        {
            myTMP.text = args.text;
            Debug.Log(args.text);
        }


        public enum State
        {
            CharacterControl,
            DialogControl,
            Pause
        }

        State state;

        public void ChangeState(State state) => this.state = state;

        void Update()
        {
            switch (state)
            {
                case State.CharacterControl:
                    CharacterControl();
                    break;
                case State.DialogControl:
                    DialogControl();
                    break;
            }
        }

        void DialogControl()
        {

            model.player.nextMoveCommand = Vector3.zero;
            if (myTMP.text =="left")
                model.dialog.FocusButton(-1);
            else if (myTMP.text =="right")
                model.dialog.FocusButton(+1);
            if (myTMP.text =="yes")
                model.dialog.SelectActiveButton();
        }

        void CharacterControl()
        {   
            if (myTMP.text =="up")
                model.player.nextMoveCommand = Vector3.up * stepSize;
            else if (myTMP.text =="down")
                model.player.nextMoveCommand = Vector3.down * stepSize;
            else if (myTMP.text =="left")
                model.player.nextMoveCommand = Vector3.left * stepSize;
            else if (myTMP.text =="right")
                model.player.nextMoveCommand = Vector3.right * stepSize;
            else if(myTMP.text == "stop")
                model.player.nextMoveCommand = Vector3.zero;
            else 
                model.player.nextMoveCommand = Vector3.zero;
        }
    }
}