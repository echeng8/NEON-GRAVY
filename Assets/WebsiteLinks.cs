using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebsiteLinks : MonoBehaviour
{
    public Button officialWebsiteButton;
    public Button redditButton;
    public Button twitterButton;
    public Button appleButton;
    public Button googleButton;
    public Button discordButton;

    public string officialWebsiteURL;
    public string redditURL;
    public string twitterURL;
    public string appleURL;
    public string googleURL;
    public string discordURL;

    
    void Start()
    {
        officialWebsiteButton.onClick.AddListener(officialLink);
        redditButton.onClick.AddListener(redditLink);
        twitterButton.onClick.AddListener(twitterLink);
        appleButton.onClick.AddListener(appleLink);
        googleButton.onClick.AddListener(googleLink);
        discordButton.onClick.AddListener(discordLink);
    }

    void officialLink()
    {
        Application.OpenURL(officialWebsiteURL);
    }
    void redditLink()
    {
        Application.OpenURL(redditURL);
    }
    void twitterLink()
    {
        Application.OpenURL(twitterURL);
    }
    void appleLink()
    {
        Application.OpenURL(appleURL);
    }

    void googleLink()
    {
        Application.OpenURL(googleURL);
    }

    void discordLink()
    {
        Application.OpenURL(discordURL);
    }
}
