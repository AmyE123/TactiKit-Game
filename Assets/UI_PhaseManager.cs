namespace CT6GAMAI
{
    using DG.Tweening;
    using System.Collections;
    using System.Text.RegularExpressions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using static CT6GAMAI.Constants;

    public class UI_PhaseManager : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _phaseText;
        [SerializeField] private Image _vignette;

        public void DisplayPhaseUI(Phases phase)
        {
            PopulatePhaseUI(phase);
            StartCoroutine(AnimatePhaseUI(2f));
        }

        private void PopulatePhaseUI(Phases phase)
        {
            var formattedPhaseName = Regex.Replace(phase.ToString(), "(\\B[A-Z])", " $1");

            _phaseText.text = formattedPhaseName;

            if (phase == Phases.PlayerPhase)
            {
                _phaseText.color = UI_PlayerColour;
                _vignette.color = UI_PlayerColour;
            }
            else
            {
                _phaseText.color = UI_EnemyColour;
                _vignette.color = UI_EnemyColour;
            }
        }

        private IEnumerator AnimatePhaseUI(float delay)
        {
            ShowPhaseUI();

            // Here you might play an animation or transition effect
            yield return new WaitForSeconds(delay);

            HidePhaseUI();
        }

        private void ShowPhaseUI()
        {
            _canvasGroup.DOFade(1, 1f);
        }

        private void HidePhaseUI()
        {
            _canvasGroup.DOFade(0, 1f);
        }
    }

}