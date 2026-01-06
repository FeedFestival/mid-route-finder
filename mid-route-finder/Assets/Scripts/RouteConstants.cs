using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using CITY = CityConstants;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class RouteConstants : MonoBehaviour {
    public static readonly Dictionary<string, string[]> BANNED_ROUTES = new() {
        {
            CITY.GALATI, new[] { CITY.BUZAU }
        }, {
            CITY.SUCEAVA, new[] { CITY.BACAU }
        }, {
            CITY.CRAIOVA, new[] { CITY.PITESTI }
        }, {
            CITY.ARAD, new[] { CITY.RESITA }
        }, {
            CITY.BRASOV, new[] { CITY.MIERCUREA_CIUC }
        }, {
            CITY.DEVA, new[] { CITY.RESITA }
        }, {
            CITY.SIBIU, new[] { CITY.DEVA }
        }, {
            CITY.SLATINA, new[] { CITY.ALEXANDRIA, CITY.RAMNICU_VALCEA }
        }
    };

    public static readonly List<requiredRoute> REQUIRED_ROUTES = new() {
        new requiredRoute {
            FromCityName = CITY.CRAIOVA,
            ToCityName = CITY.ALEXANDRIA,
        },
        new requiredRoute {
            FromCityName = CITY.ARAD,
            ToCityName = CITY.DEVA,
        },
        new requiredRoute {
            FromCityName = CITY.ORADEA,
            ToCityName = CITY.DEVA,
        },
        new requiredRoute {
            FromCityName = CITY.BRASOV,
            ToCityName = CITY.SIBIU,
        },
        new requiredRoute {
            FromCityName = CITY.FOCSANI,
            ToCityName = CITY.SFANTU_GHEORGHE,
        },
        new requiredRoute {
            FromCityName = CITY.GALATI,
            ToCityName = CITY.VASLUI,
        },
        new requiredRoute {
            FromCityName = CITY.PIATRA_NEAMT,
            ToCityName = CITY.BISTRITA,
        },
        new requiredRoute {
            FromCityName = CITY.CALARASI,
            ToCityName = CITY.GIURGIU,
        },
        new requiredRoute {
            FromCityName = CITY.SIBIU,
            ToCityName = CITY.TARGU_JIU,
        },
        new requiredRoute {
            FromCityName = CITY.BRASOV,
            ToCityName = CITY.RAMNICU_VALCEA,
        },
        new requiredRoute {
            FromCityName = CITY.TARGU_JIU,
            ToCityName = CITY.RESITA,
        },
        new requiredRoute {
            FromCityName = CITY.SUCEAVA,
            ToCityName = CITY.BISTRITA,
        },
        new requiredRoute {
            FromCityName = CITY.BISTRITA,
            ToCityName = CITY.MIERCUREA_CIUC
        },
        new requiredRoute {
            FromCityName = CITY.CLUJ_NAPOCA,
            ToCityName = CITY.DEVA
        },
    };

    public static readonly Dictionary<string, citySettings> CITY_SETTINGS = new() {
        { // 13
            CITY.GALATI, new() {
                TextAlignmentOptions = TextAlignmentOptions.Top
            }
        }, { // 14
            CITY.TARGOVISTE, new() {
                TextAlignmentOptions = TextAlignmentOptions.Top
            }
        }, { // 19
            CITY.BOTOSANI, new() {
                TextAlignmentOptions = TextAlignmentOptions.Top
            }
        }, { // 24
            CITY.RAMNICU_VALCEA, new() {
                TextAlignmentOptions = TextAlignmentOptions.Top
            }
        }, { // 26
            CITY.SATU_MARE, new() {
                TextAlignmentOptions = TextAlignmentOptions.Top
            }
        }, { // 28
            CITY.ALEXANDRIA, new() {
                TextAlignmentOptions = TextAlignmentOptions.Top
            }
        }
    };

    public static readonly Dictionary<string, Dictionary<string, routeSettings>> ROUTES_SETTINGS = new() {
        { // 0
            CITY.BUCURESTI, new() {
                {
                    CITY.PLOIESTI, new() {
                        TwoWay = true,
                    }
                }, {
                    CITY.GIURGIU, new() {
                        TwoWay = true,
                        Color = RouteColor.Orange,
                        SecondColor = RouteColor.Pink
                    }

                    // }, {
                    //     CITY.BUZAU, new() {
                    //         Color = RouteColor.White,
                    //     }
                }, {
                    CITY.ALEXANDRIA, new() {
                        Color = RouteColor.Red,
                    }
                }, {
                    CITY.SLOBOZIA, new() {
                        EnforcedLength = 3,
                        TwoWay = true,
                        Color = RouteColor.Blue,
                        SecondColor = RouteColor.Yellow
                    }
                }, {
                    CITY.TARGOVISTE, new() {
                        Color = RouteColor.Green,
                    }
                },
            }
        },

        // 1
        {
            CITY.IASI, new() {
                {
                    CITY.VASLUI, new() {
                        TwoWay = true,
                        Color = RouteColor.Yellow,
                        SecondColor = RouteColor.Red,
                    }
                }, {
                    CITY.BACAU, new() {
                        Color = RouteColor.Blue,
                    }
                },
            }
        },

        // 2
        {
            CITY.PLOIESTI, new() {
                {
                    CITY.TARGOVISTE, new() {
                        EnforcedLength = 1,
                        EnforcedPlaceholderSizeRatio = 1
                    }
                }, {
                    CITY.BUZAU, new() {
                        EnforcedLength = 2
                    }
                }, {
                    CITY.BRASOV, new() {
                        TwoWay = true
                    }
                }
            }
        },

        // 3
        {
            CITY.CLUJ_NAPOCA, new() {
                {
                    CITY.ZALAU, new() {
                        EnforcedLength = 2,
                        TwoWay = true,
                        Color = RouteColor.Black,
                        SecondColor = RouteColor.Blue,
                    }
                }, {
                    CITY.DEVA, new() {
                        EnforcedLength = 6,
                        Color = RouteColor.Red,
                    }
                }, {
                    CITY.BISTRITA, new() {
                        EnforcedLength = 3,
                        TwoWay = true,
                    }
                }, {
                    CITY.ALBA_IULIA, new() {
                        EnforcedLength = 3,
                        Color = RouteColor.Yellow,
                    }
                }
            }
        },

        // 4
        {
            CITY.CONSTANTA, new() {
                {
                    CITY.CALARASI, new() {
                        Color = RouteColor.Red
                    }
                }, {
                    CITY.SLOBOZIA, new() {
                        Color = RouteColor.Blue
                    }
                }, {
                    CITY.TULCEA, new() {
                        Color = RouteColor.Pink
                    }
                }
            }
        },

        // 5
        {
            CITY.TIMISOARA, new() {
                {
                    CITY.DEVA, new() {
                        TwoWay = true,
                        Color = RouteColor.White,
                        SecondColor = RouteColor.Orange
                    }
                },
                {
                    CITY.ARAD, new() {
                        TwoWay = true,
                    }
                }
            }
        },

        // 6
        {
            CITY.SUCEAVA, new() {
                {
                    CITY.BISTRITA, new() {
                        TwoWay = true,
                        Color = RouteColor.Black,
                        SecondColor = RouteColor.Pink
                    }
                }
            }
        },

        // 7
        {
            CITY.BACAU, new() {
                {
                    CITY.VASLUI, new() {
                        TwoWay = true,
                        Color = RouteColor.Green,
                        SecondColor = RouteColor.White
                    }
                }, {
                    CITY.MIERCUREA_CIUC, new() {
                        TwoWay = true,
                        Color = RouteColor.Orange,
                        SecondColor = RouteColor.Black
                    }
                }
            }
        },

        // 8
        {
            CITY.CRAIOVA, new() {
                {
                    CITY.ALEXANDRIA, new() {
                        Color = RouteColor.Black
                    }
                }, {
                    CITY.RAMNICU_VALCEA, new() {
                        Color = RouteColor.Yellow
                    }
                }
            }
        },

        // 9
        {
            CITY.PITESTI, new() {
                {
                    CITY.SLATINA, new() {
                        // Color = RouteColor.Red
                    }
                }, {
                    CITY.TARGOVISTE, new() {
                        EnforcedLength = 1,
                        EnforcedPlaceholderSizeRatio = 1
                    }
                }, {
                    CITY.RAMNICU_VALCEA, new() {
                        EnforcedLength = 1,
                        EnforcedPlaceholderSizeRatio = 1
                    }
                },
            }
        },

        // 10
        {
            CITY.ORADEA, new() {
                {
                    CITY.ARAD, new() {
                        TwoWay = true,
                        Color = RouteColor.Green,
                        SecondColor = RouteColor.Pink,
                    }
                }, {
                    CITY.DEVA, new() {
                        Color = RouteColor.Blue,
                    }
                }, {
                    CITY.ZALAU, new() {
                        Color = RouteColor.White,
                    }
                },
            }
        },

        // 11
        {
            CITY.BRASOV, new() {
                {
                    CITY.RAMNICU_VALCEA, new() {
                        Color = RouteColor.Blue,
                    }
                }, {
                    CITY.SFANTU_GHEORGHE, new() {
                        TwoWay = true,
                        EnforcedPlaceholderSizeRatio = 0.9f
                    }
                }
            }
        },

        // 12
        {
            CITY.TARGU_MURES, new() {
                {
                    CITY.MIERCUREA_CIUC, new() {
                        EnforcedLength = 4,
                        Color = RouteColor.Green
                    }
                }, {
                    CITY.ALBA_IULIA, new() {
                        TwoWay = true,
                        Color = RouteColor.Black,
                        SecondColor = RouteColor.Orange,
                    }
                }, {
                    CITY.SIBIU, new() {
                        Color = RouteColor.Pink,
                    }
                }
            }
        },

        // 13
        {
            CITY.GALATI, new() {
                {
                    CITY.BRAILA, new() {
                        EnforcedPlaceholderSizeRatio = 0.8f
                    }
                }, {
                    CITY.TULCEA, new() {
                        TwoWay = true,
                        Color = RouteColor.Orange,
                        SecondColor = RouteColor.Black,
                    }
                }, {
                    CITY.VASLUI, new() {
                        Color = RouteColor.White,
                    }
                },
            }
        },

        // 15
        {
            CITY.PIATRA_NEAMT, new() {
                {
                    CITY.BISTRITA, new() {
                        EnforcedLength = 6,
                        TwoWay = true,
                        Color = RouteColor.Yellow,
                        SecondColor = RouteColor.Orange,
                    }
                }, {
                    CITY.BOTOSANI, new() {
                        EnforcedLength = 4,
                        TwoWay = true,
                        Color = RouteColor.Red,
                        SecondColor = RouteColor.Blue,
                    }
                }, {
                    CITY.MIERCUREA_CIUC, new() {
                        TwoWay = true,
                    }
                }
            }
        },

        // 16
        {
            CITY.BAIA_MARE, new() {
                {
                    CITY.BISTRITA, new() {
                        TwoWay = true
                    }
                },
                {
                    CITY.SATU_MARE, new() {
                        TwoWay = true
                    }
                }
            }
        },

        // 18
        {
            CITY.BUZAU, new() {
                {
                    CITY.BRAILA, new() {
                        Color = RouteColor.Black
                    }
                }, {
                    CITY.FOCSANI, new() {
                        TwoWay = true
                    }
                }, {
                    CITY.SLOBOZIA, new() {
                        Color = RouteColor.Pink
                    }
                },
            }
        },

        // 20
        {
            CITY.SIBIU, new() {
                {
                    CITY.TARGU_JIU, new() {
                        Color = RouteColor.White
                    }
                },
            }
        },

        // 24
        {
            CITY.RAMNICU_VALCEA, new() {
                {
                    CITY.TARGU_JIU, new() {
                        Color = RouteColor.Red,
                    }
                }
            }
        },

        // 25
        {
            CITY.FOCSANI, new() {
                {
                    CITY.SFANTU_GHEORGHE, new() {
                        EnforcedLength = 5,
                        Color = RouteColor.Yellow,
                    }
                }
            }
        },

        // 26
        {
            CITY.SATU_MARE, new() {
                {
                    CITY.ZALAU, new() {
                        Color = RouteColor.Orange,
                    }
                }
            }
        },

        // 28
        {
            CITY.ALEXANDRIA, new() {
                {
                    CITY.GIURGIU, new() {
                        EnforcedLength = 1,
                        EnforcedPlaceholderSizeRatio = 0.8f
                    }
                }
            }
        },

        // 30
        {
            CITY.BISTRITA, new() {
                {
                    CITY.ZALAU, new() {
                        EnforcedLength = 5,
                        TwoWay = true,
                        Color = RouteColor.Green,
                        SecondColor = RouteColor.Red
                    }
                }, {
                    CITY.MIERCUREA_CIUC, new() {
                        EnforcedLength = 6,
                        Color = RouteColor.White,
                    }
                }
            }
        },

        // 31
        {
            CITY.MIERCUREA_CIUC, new() {
                {
                    CITY.SFANTU_GHEORGHE, new() {
                        TwoWay = true,
                        Color = RouteColor.Green,
                        SecondColor = RouteColor.White,
                    }
                }
            }
        },

        // 32
        {
            CITY.CALARASI, new() {
                {
                    CITY.GIURGIU, new() {
                        EnforcedLength = 6,
                        Color = RouteColor.Green
                    }
                }, { // Calarasi -> Slobozia
                    CITY.SLOBOZIA, new() {
                        EnforcedPlaceholderSizeRatio = 1
                    }
                }
            }
        },

        // 33
        {
            CITY.BRAILA, new() {
                {
                    CITY.SLOBOZIA, new() {
                        TwoWay = true
                    }
                },
            }
        },

        // 36 Resita -> Drobeta Turnu Severin
        {
            CITY.RESITA, new() {
                {
                    CITY.DROBETA_TURNU_SEVERIN, new() {
                        TwoWay = true,
                        Color = RouteColor.Yellow,
                        SecondColor = RouteColor.Pink
                    }
                },
            }
        }
    };
}

public enum RouteColor {
    Gray,
    Red,
    Blue,
    Green,
    Yellow,
    Orange,
    Pink,
    Black,
    White
}

public struct requiredRoute {
    public string FromCityName;
    public string ToCityName;
}

public struct citySettings {
    public TextAlignmentOptions TextAlignmentOptions;
}

public struct routeSettings {
    public bool TwoWay;
    public float? EnforcedPlaceholderSizeRatio;
    public int EnforcedLength;
    public RouteColor Color;
    public RouteColor? SecondColor;
}
