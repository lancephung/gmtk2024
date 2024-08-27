using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCounter : MonoBehaviour
{
    [SerializeField] private List<Image> _moveIndicators;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _inactiveColor = new Color(0, 0, 0, .2f);
    [SerializeField] private Color _warningColor = new Color(255, 150, 150);

    public static MoveCounter Instance;

    private Coroutine _animation;

    // Update is called once per frame
    private void Awake()
    {
        Instance = this;
    }

    public void IndicateMoves(int moves, bool isWarning = false)
    {
        //if (_animation != null)
        //{
        //    StopCoroutine(_animation);
        //}
        //if (moves == 0 && isWarning)
        //{
        //    IEnumerator Delay()
        //    {
        //        for (int i = 0; i < 2; i++)
        //        {
        //            _moveIndicators.ForEach(img => img.color = _warningColor);
        //            yield return new WaitForSeconds(.25f);
        //            _moveIndicators.ForEach(img => img.color = _inactiveColor);
        //            yield return new WaitForSeconds(.25f);
        //        }
        //    }
        //    _animation = StartCoroutine(Delay());
        //    return;
        //}
        //for (int i = 0; i < _moveIndicators.Count; i++)
        //{
        //    _moveIndicators[i].color = moves > i ? _activeColor : _inactiveColor;
        //}
    }

}
