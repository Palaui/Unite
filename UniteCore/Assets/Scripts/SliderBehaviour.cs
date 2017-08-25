using UnityEngine;
using UnityEngine.UI;

public class SliderBehaviour : MonoBehaviour
{
    public Test test;

    public void ChangeValue()
    {
        test.ChangeRFromSLider(GetComponent<Slider>().value);
    }
}
