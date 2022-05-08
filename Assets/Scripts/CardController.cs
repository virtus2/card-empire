using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public GameManager gameManager;
    [Header("텍스트")]
    public TextMeshProUGUI topText;
    public TextMeshProUGUI bottomText;
    [Header("움직이는 카드")]
    public GameObject movingCard;
    public TextMeshProUGUI movingCardLeftText;
    public TextMeshProUGUI movingCardRightText;
    public Image movingCardImage;

    [Header("고정된 카드")]
    public TextMeshProUGUI cardDeckLeftText;
    public TextMeshProUGUI cardDeckRightText;
    public Image cardDeckImage;
    public GameObject cardDeck;
    public GameObject frontside;
    public GameObject backside;
    private SpriteRenderer renderer;
    private const float X_MAX = 4.5f;
    private const float Z_ROTATION_MAX = 30f;
    private const float CardMovingSpeed = 0.5f;
    private const float DeadZone = 0.5f;

    private float inputMag;
    private Vector3 downPos;
    private Vector3 upPos;
    private bool animating = false;
    private Vector3 topTextBasePosition;
    private Color topTextBaseColor;
    private Vector3 basePosition = new Vector3(0, 0, 0);
    private Quaternion baseQuaternion = Quaternion.Euler(0, 0, 0);
    private Quaternion half = Quaternion.Euler(0, 90, 0);


    private void Start()
    {
        topTextBasePosition = topText.transform.position;
        topTextBaseColor = topText.color;
    }
    public void ResetPosition()
    {
        StartCoroutine(SmoothMove(movingCard.transform.position, basePosition, 0.2f));
        StartCoroutine(SmoothRotate(movingCard.transform.rotation, baseQuaternion, 0.2f));
        //cardObject.transform.position = Vector2.MoveTowards(cardObject.transform.position, basePosition, 0);
        //cardObject.transform.rotation = baseQuaternion;
    }
    private IEnumerator SmoothMove(Vector2 startPos, Vector2 endPos, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            movingCard.transform.position = Vector2.MoveTowards(startPos, endPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;

        }
    }
    private IEnumerator SmoothRotate(Quaternion start, Quaternion end, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            movingCard.transform.rotation = Quaternion.Lerp(start, end, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    
    public void StartCardUpdate(CardData nextCard)
    {
        // 카드를 뒤집음
        StartFlipBackgroundCard(nextCard);
    }
    public void StartFlipBackgroundCard(CardData nextCard)
    {
        backside.SetActive(true);
        frontside.SetActive(false);
        StartCoroutine(FlipDeck(nextCard, 0.5f));
        StartCoroutine(UpdateText(nextCard, 1f));
    }
    public void UpdateMovingCardData(CardData data)
    {
        backside.SetActive(true);
        frontside.SetActive(false);
        movingCard.transform.position = basePosition;
        movingCard.transform.rotation = baseQuaternion;
        movingCardLeftText.text = data.answerYesKor;
        movingCardRightText.text = data.answerNoKor;
        movingCardImage.sprite = SpriteManager.Instance.GetSpriteByName(data.teller);

        movingCardLeftText.alpha = 0;
        movingCardRightText.alpha = 0;
        topText.text = data.textKor;
    }

    private IEnumerator FlipDeck(CardData nextCard, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time / 2)
        {
            cardDeck.transform.rotation = Quaternion.Lerp(cardDeck.transform.rotation, half, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        backside.SetActive(false);
        frontside.SetActive(true);
        cardDeckImage.sprite = SpriteManager.Instance.GetSpriteByName(nextCard.teller);
        StartCoroutine(FlipFront(nextCard, time /2));
    }
    private IEnumerator FlipFront(CardData nextCard, float time)
    {
        //cardDeckLeftText.text = nextCard.answerYesKor;
        //cardDeckRightText.text = nextCard.answerNoKor;

        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            cardDeck.transform.rotation = Quaternion.Lerp(cardDeck.transform.rotation, baseQuaternion, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        UpdateMovingCardData(nextCard);
    }
    private IEnumerator UpdateText(CardData nextCard, float time)
    {
        bottomText.alpha = 0;
        bottomText.text = TextManager.Instance.GetTextByName(nextCard.teller);
        topText.alpha = 0;
        topText.text = nextCard.textKor;
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            topText.color = Color.Lerp(topText.color, topTextBaseColor, elapsedTime / time);
            bottomText.color = Color.Lerp(bottomText.color, topTextBaseColor, elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.position);
        inputMag = Vector2.Distance(downPos, pos);
        if (inputMag < 0.07f) return;
        pos.Normalize();
        pos.y = 0;
        movingCard.transform.position = pos;
        movingCard.transform.eulerAngles = new Vector3(0, 0, -pos.x * 5);


        Debug.Log(pos.x);
        if (movingCard.transform.position.x > 0.001f)
        {
            movingCardRightText.alpha = pos.x;
        }
        else if(movingCard.transform.position.x < -0.001f)
        {
            movingCardLeftText.alpha = -pos.x;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        downPos = Camera.main.ScreenToWorldPoint(eventData.position);
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputMag < 0.07f) return;
        if (movingCard.transform.position.x < -0.7)
        {
            gameManager.OnLeftSwipe();
            StartCoroutine(ThrowMovingCard(0.3f));
        }
        else if(movingCard.transform.position.x > 0.7)
        {
            gameManager.OnRightSwipe();
            StartCoroutine(ThrowMovingCard(0.3f));
        }
        else
        {
            ResetPosition();
        }
    }

    private IEnumerator ThrowMovingCard(float time)
    {
        animating = true;
        float elapsedTime = 0f;
        Vector3 fallPos = new Vector3(movingCard.transform.position.x * 500, 200, 0);
        Quaternion fallAngle = Quaternion.Euler(0, 0, -movingCard.transform.position.x * 20);
        while(elapsedTime < time)
        {

            movingCard.transform.position = Vector2.MoveTowards(movingCard.transform.position, fallPos, elapsedTime / time);
            movingCard.transform.rotation = Quaternion.Lerp(movingCard.transform.rotation, fallAngle, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }
}
