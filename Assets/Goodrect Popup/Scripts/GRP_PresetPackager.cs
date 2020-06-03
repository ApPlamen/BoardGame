using UnityEngine;

public class GRP_PresetPackager
{
    //Colors
    public Color bg_tint;
    public Color container_tint;
    public Color prompt_inputfield_tint;

    public Color return_tint;
    public Color btn_tint;
    public Color btn_pressed_tint;
    public Color return_pressed_tint;

    public Color title_text_color;
    public Color body_text_color;
    public Color btn_text_color;
    public Color return_text_color;
    public Color btn_text_pressed_color;
    public Color return_text_pressed_color;

    public Color prompt_inputfield_text_color;
    public Color prompt_placeholder_text_color;

    //Textures
    public bool prefix_image;
    public string bg_image;
    public string container_image;
    public string btn_image;
    public string btn_pressed_image;
    public string rtn_btn_image;
    public string rtn_btn_pressed_image;
    public string prompt_inputfield_image;

    public int container_bleed;
    public int btn_bleed;

    //Typgraphy
    public int title_text_size;
    public int body_text_size;
    public int button_text_size;

    public string title_font;
    public string body_font;
    public string button_font;
    public string return_font;

    public TextAlignment alignment;

    //Fade
    public FadeAnimation fade_animation;
    public AnimationCurve fade_curve;
    public float fade_dur;
}
