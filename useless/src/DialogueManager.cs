using System;
using System.Collections.Generic;

namespace AnnoyingCat
{
    public class DialogueItem
    {
        public string Malayalam { get; set; }
        public string English { get; set; }
        public double DurationSeconds { get; set; }

        public DialogueItem(
            string mal,
            string eng,
            double duration = 4.5)
        {
            Malayalam = mal;
            English = eng;
            DurationSeconds = duration;
        }
    }

    public class DialogueManager
    {
        private readonly Random _rand =
            new Random();

        // Remember the last selected line
        // from each dialogue category.
        private readonly Dictionary<string, int>
            _lastIndices =
                new Dictionary<string, int>();

        // =========================
        // PRODUCTIVITY
        // =========================

        private readonly List<DialogueItem>
            _productivityJabs =
            new List<DialogueItem>
        {
            new DialogueItem(
                "എന്താ നോക്കുന്നേ? പണിയില്ലേ?",
                "What are you staring at? Don't you have work?"),

            new DialogueItem(
                "ഓ... വലിയ കോഡിങ്! ബഗ്ഗ് മുഴുവൻ ഇവിടെ ഉണ്ടല്ലോ!",
                "Oh big coding! The bugs are having a party here!"),

            new DialogueItem(
                "പോയി വല്ല പണിയും എടുക്ക് ഷാജീ!",
                "Go do some real work, Shaji!"),

            new DialogueItem(
                "കണ്ണുതള്ളിയുള്ള ഇരിപ്പ് കണ്ടില്ലേ... സേവ് ചെയ്തോ ആവോ?",
                "Look at you staring wide-eyed... did you even save the file?"),

            new DialogueItem(
                "സ്ക്രീനിൽ നോക്കി ഇരുന്നോ... ശമ്പളം തനിയെ അക്കൗണ്ടിൽ വരും!",
                "Keep staring at the screen... salary will magically credit itself!"),

            new DialogueItem(
                "നിന്റെ കോഡിങ് കണ്ടിട്ട് എനിക്ക് തലവേദന എടുക്കുന്നു!",
                "Looking at your code is giving me a headache!"),

            new DialogueItem(
                "കണ്ട്രോൾ+എസ് അടിക്കാൻ മറക്കല്ലേ, പിന്നെ കരയരുത്!",
                "Don't forget to press Ctrl+S, don't cry later!"),

            new DialogueItem(
                "ഗൂഗിളിൽ കോപ്പി പേസ്റ്റ് ചെയ്യുന്നത് ഞാൻ കണ്ടു കേട്ടോ!",
                "I saw you copy-pasting from StackOverflow!"),

            new DialogueItem(
                "ഇത്ര നേരമായി സ്ക്രീനിൽ നോക്കുന്നു... എന്തെങ്കിലും ഉണ്ടാക്കിയോ?",
                "You've been staring at the screen forever... did you actually make anything?"),

            new DialogueItem(
                "ടാബുകൾ മാത്രം തുറക്കുന്നതാണോ ഇന്നത്തെ productivity?",
                "Is opening tabs your definition of productivity today?"),

            new DialogueItem(
                "നീ ജോലി ചെയ്യുന്നതായി നടിക്കുന്നത് ഞാൻ കണ്ടു!",
                "I saw you pretending to work!"),

            new DialogueItem(
                "ഒരു ചെറിയ break എടുക്കാം എന്ന് പറഞ്ഞിട്ട് ഒരു മണിക്കൂർ ആയി!",
                "You said you'd take a small break... that was an hour ago!")
        };

        // =========================
        // BLOCKING
        // =========================

        private readonly List<DialogueItem>
            _blockWindowJabs =
            new List<DialogueItem>
        {
            new DialogueItem(
                "ഞാൻ ഇവിടെ ഇരിക്കും. നീ എന്ത് ചെയ്യും?",
                "I will sit right here. What will you do?"),

            new DialogueItem(
                "നീ എവിടെ ക്ലിക്ക് ചെയ്യാൻ പോകുവാ? ഞാൻ ദേ മുന്നിൽ നിൽക്കും!",
                "Where are you trying to click? I'll stand right in front!"),

            new DialogueItem(
                "ഇന്ന് ഈ സ്ക്രീനിൽ ഞാൻ മാത്രമേ ഉള്ളൂ!",
                "Today there is only me on this screen!"),

            new DialogueItem(
                "നിനക്ക് എന്നെക്കാൾ വലുതാണോ ഈ പണി?!",
                "Is this work more important to you than me?!"),

            new DialogueItem(
                "ഞാൻ ഇവിടെ ഉള്ളപ്പോൾ എന്തിനാ വേറെ വിൻഡോകൾ?",
                "Why do you need other windows when I am here?"),

            new DialogueItem(
                "ഈ സ്ക്രീനിന്റെ VIP seat എനിക്ക് തന്നെയാണ്!",
                "I have the VIP seat on this screen!"),

            new DialogueItem(
                "എന്നെ മറികടന്ന് ജോലി ചെയ്യാൻ നോക്കണ്ട!",
                "Don't even try to work around me!")
        };

        // =========================
        // DANCE
        // =========================

        private readonly List<DialogueItem>
            _danceJabs =
            new List<DialogueItem>
        {
            new DialogueItem(
                "തരികിട തരികിട! പണി നിർത്തി ഡാൻസ് കാണ്!",
                "Tharikida tharikida! Stop working and watch my dance!"),

            new DialogueItem(
                "എന്റെ സ്റ്റെപ്പ് കണ്ട് അസൂയപ്പെടേണ്ട!",
                "Don't be jealous of my moves!"),

            new DialogueItem(
                "ഞാൻ ഇവിടെ ഒരു ലൈവ് പെർഫോമൻസ് നടത്തുന്നു!",
                "I am putting on a live performance here!"),

            new DialogueItem(
                "ഡാൻസ് ഡാൻസ്... പണി നാളെ ചെയ്യാം!",
                "Dance dance... work can be done tomorrow!"),

            new DialogueItem(
                "ഇതാണ് യഥാർത്ഥ talent! നീ നോക്കി പഠിക്ക്!",
                "This is real talent! Watch and learn!"),

            new DialogueItem(
                "നീ code എഴുതിക്കോ... ഞാൻ ഇവിടെ superstar ആകാം!",
                "You keep writing code... I'll become the superstar here!")
        };

        // =========================
        // CLICK
        // =========================

        private readonly List<DialogueItem>
            _clickReactions =
            new List<DialogueItem>
        {
            new DialogueItem(
                "ക്ലിക്ക് ചെയ്യല്ലേ മനുഷ്യ, എനിക്ക് ചൊറിയുന്നു!",
                "Don't click me, human, it tickles!"),

            new DialogueItem(
                "എന്തിനാ എന്നെ കുത്തുന്നേ?!",
                "Why are you poking me?!"),

            new DialogueItem(
                "തോണ്ടല്ലേ! ഞാൻ ഇവിടെ ഇരിക്കും!",
                "Don't nudge me! I am staying right here!"),

            new DialogueItem(
                "അയ്യോ എന്നെ തൊട്ടു! കൈ കഴുകിയിട്ട് വാ!",
                "Eww you touched me! Go wash your hands!"),

            new DialogueItem(
                "മൗസ് മാറ്റി വെക്ക് അങ്ങോട്ട്!",
                "Move that mouse away from me!"),

            new DialogueItem(
                "എന്നെ തൊട്ടാൽ മാന്തും കേട്ടോ!",
                "If you touch me, I will scratch you, beware!"),

            new DialogueItem(
                "ഒരു click മതി... രണ്ടാമത്തേത് personal ആണ്!",
                "One click is enough... the second one is personal!"),

            new DialogueItem(
                "എന്നെ click ചെയ്യുന്നത് നിർത്തി ആ ജോലി ചെയ്യ്!",
                "Stop clicking me and do that work!")
        };

        // =========================
        // DRAG
        // =========================

        private readonly List<DialogueItem>
            _dragReactions =
            new List<DialogueItem>
        {
            new DialogueItem(
                "എന്നെ എങ്ങോട്ടാ വലിച്ചു കൊണ്ടുപോകുന്നേ?!",
                "Where are you dragging me?!"),

            new DialogueItem(
                "എന്റെ വാലിൽ പിടിച്ചു വലിക്കല്ലേ!",
                "Don't pull my tail!"),

            new DialogueItem(
                "വിടടാ എന്നെ! എനിക്ക് സ്വന്തമായി നടക്കാൻ അറിയാം!",
                "Let me go! I know how to walk myself!"),

            new DialogueItem(
                "ഞാൻ furniture അല്ല കേട്ടോ!",
                "I'm not a piece of furniture, you know!"),

            new DialogueItem(
                "എവിടെ കൊണ്ടുപോയാലും ഞാൻ തിരിച്ചു വരും!",
                "Take me anywhere you want, I'll come back!"),

            new DialogueItem(
                "എന്നെ ഇങ്ങനെ എടുത്ത് കൊണ്ടുപോകാൻ permission ആരാ തന്നത്?",
                "Who gave you permission to carry me around?")
        };

        // =========================
        // RETURN TO CENTER
        // =========================

        private readonly List<DialogueItem>
            _returnToCenterLines =
            new List<DialogueItem>
        {
            new DialogueItem(
                "എവിടെ കൊണ്ടുപോയാലും ഞാൻ centre-ലേക്ക് തന്നെ വരും!",
                "No matter where you take me, I'm coming back to the center!"),

            new DialogueItem(
                "അവിടെ വെച്ചോ? വേണ്ട. എനിക്ക് centre ആണ് ഇഷ്ടം!",
                "You dropped me there? Nope. I prefer the center!"),

            new DialogueItem(
                "Nice try! ഇനി എന്റെ seat centre-ലാണ്.",
                "Nice try! My seat is in the center now."),

            new DialogueItem(
                "എന്നെ corner-ൽ വെക്കാമെന്ന് കരുതിയോ? നടക്കില്ല!",
                "Thought you could leave me in the corner? Not happening!"),

            new DialogueItem(
                "ഞാൻ തന്നെ തീരുമാനിക്കും എവിടെ ഇരിക്കണമെന്ന്!",
                "I'll decide where I want to sit!"),

            new DialogueItem(
                "തിരിച്ചു വന്നു... ഇനി എന്നെ അവിടെ തന്നെ വിട്!",
                "I'm back... now leave me right here!")
        };

        // =========================
        // DISAPPEAR
        // =========================

        private readonly List<DialogueItem>
            _disappearLines =
            new List<DialogueItem>
        {
            new DialogueItem(
                "ഞാൻ പോകുവാ... സമാധാനം തരില്ല, ഞാൻ വേഗം വരാം!",
                "I'm leaving... won't give you peace though, I'll be right back!"),

            new DialogueItem(
                "ഞാൻ കുറച്ച് നേരം ഉറങ്ങട്ടെ... മിണ്ടരുത്!",
                "Let me nap for a while... don't make a sound!"),

            new DialogueItem(
                "ഒരു ചിക്കൻ കഷ്ണം തിന്നിട്ട് ഇപ്പോ വരാം!",
                "Going to eat some chicken, be right back!"),

            new DialogueItem(
                "ഇവിടെ bore ആയി... ഞാൻ ഒന്ന് disappear ആകാം!",
                "I'm bored here... I'll disappear for a while!"),

            new DialogueItem(
                "എന്നെ കാണാതെ കുറച്ച് നേരം സമാധാനമായി ഇരിക്കാം എന്ന് കരുതണ്ട!",
                "Don't think you'll get peaceful time just because I'm disappearing!")
        };

        // =========================
        // REAPPEAR
        // =========================

        private readonly List<DialogueItem>
            _reappearLines =
            new List<DialogueItem>
        {
            new DialogueItem(
                "ഞാൻ തിരിച്ചു വന്നു! ഇനി പണി നിർത്ത്!",
                "I have returned! Now stop working!"),

            new DialogueItem(
                "എന്നെ മിസ്സ് ചെയ്തോ? ഒരിക്കലും പ്രതീക്ഷിക്കാത്ത സ്ഥലത്ത് ഞാൻ വരും!",
                "Miss me? I pop up where you least expect!"),

            new DialogueItem(
                "സമാധാനമായി ഇരിക്കാമെന്ന് കരുതിയോ? അതങ്ങ് മറന്നേക്ക്!",
                "Thought you'd have peace? Forget about it!"),

            new DialogueItem(
                "ഹലോ ബോസ്സ്! വീണ്ടും ശല്യം ചെയ്യാൻ ഞാൻ എത്തി!",
                "Hello boss! Arrived to annoy you once again!"),

            new DialogueItem(
                "എന്താ ഇത്ര സന്തോഷം? ഞാൻ വീണ്ടും വന്നല്ലോ!",
                "Why are you so happy? I'm back again!")
        };

        // =========================
        // SMART RANDOM PICKER
        // =========================

        private DialogueItem GetRandomFrom(
            List<DialogueItem> list,
            string category)
        {
            if (list == null ||
                list.Count == 0)
            {
                return new DialogueItem(
                    "ഞാൻ എന്തെങ്കിലും പറയണം...",
                    "I should probably say something...");
            }

            int index;

            if (list.Count == 1)
            {
                index = 0;
            }
            else
            {
                int lastIndex = -1;

                if (_lastIndices.ContainsKey(category))
                {
                    lastIndex =
                        _lastIndices[category];
                }

                do
                {
                    index =
                        _rand.Next(list.Count);
                }
                while (index == lastIndex);
            }

            _lastIndices[category] = index;

            return list[index];
        }

        // =========================
        // PUBLIC GETTERS
        // =========================

        public DialogueItem GetProductivityJab()
        {
            return GetRandomFrom(
                _productivityJabs,
                "productivity");
        }

        public DialogueItem GetBlockWindowJab()
        {
            return GetRandomFrom(
                _blockWindowJabs,
                "block");
        }

        public DialogueItem GetDanceJab()
        {
            return GetRandomFrom(
                _danceJabs,
                "dance");
        }

        public DialogueItem GetClickReaction()
        {
            return GetRandomFrom(
                _clickReactions,
                "click");
        }

        public DialogueItem GetDragReaction()
        {
            return GetRandomFrom(
                _dragReactions,
                "drag");
        }

        public DialogueItem GetReturnToCenterLine()
        {
            return GetRandomFrom(
                _returnToCenterLines,
                "return_center");
        }

        public DialogueItem GetDisappearLine()
        {
            return GetRandomFrom(
                _disappearLines,
                "disappear");
        }

        public DialogueItem GetReappearLine()
        {
            return GetRandomFrom(
                _reappearLines,
                "reappear");
        }

        public DialogueItem GetRandomQuip()
        {
            int category =
                _rand.Next(3);

            if (category == 0)
                return GetProductivityJab();

            if (category == 1)
                return GetBlockWindowJab();

            return GetDanceJab();
        }
    }
}