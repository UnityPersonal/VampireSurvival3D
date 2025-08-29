using DebugToolkit.Interaction.Commands;
using DebugToolkit.Console;
using UnityEngine;

namespace DebugToolkit.Sample
{
    public class DebugTimeScale : MonoBehaviour
    {
        [Header("Command")]
        [SerializeField] private VectorCommand timeScaleCommand;
        [SerializeField] private VectorCommand frameCommand;

        [Header("Control")]
        [SerializeField] private TimeControlButton pauseButton;
        [SerializeField] private TimeControlButton frameButton;


        private void Awake()
        {
            timeScaleCommand.OnVector1Inputed += HandleTimeScaleChanged;
            frameCommand.OnVector1Inputed += HandleFrameCommand;

            pauseButton.OnActivation.AddListener(delegate {
                pauseButton.Active = !pauseButton.Active;
                if(pauseButton.Active)
                {
                    HandleTimeScaleChanged(0);
                }
                else
                {
                    HandleTimeScaleChanged(1);
                }
            });
            frameButton.OnActivation.AddListener(delegate {
                frameButton.Active = !frameButton.Active;
                if(frameButton.Active)
                {
                    HandleFrameCommand(1);
                    pauseButton.Active = true;
                    pauseButton.UpdateColor();
                    frameButton.UpdateColorAsync();
                }
            });
        }

        private void OnDestroy()
        {
            timeScaleCommand.OnVector1Inputed -= HandleTimeScaleChanged;
            frameCommand.OnVector1Inputed -= HandleFrameCommand;
        }

        private void HandleTimeScaleChanged(float val)
        {
            if (this == null) return;

            Time.timeScale = val;
        }

        private async void HandleFrameCommand(float amountOfFrames)
        {
            if (this == null) return;

            Time.timeScale = 1;
            while(amountOfFrames > 0)
            {
                await Awaitable.NextFrameAsync();
                amountOfFrames--;
            }
            Time.timeScale = 0;
        }
    }
}
