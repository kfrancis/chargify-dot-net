﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace PCLWebUtility
{
    internal class ExtendedHtmlUtility
    {
        private static readonly Regex entityResolver;

        static ExtendedHtmlUtility()
        {
            ExtendedHtmlUtility.entityResolver = new Regex("([&][#](?'decimal'[0-9]+);)|([&][#][(x|X)](?'hex'[0-9a-fA-F]+);)|([&](?'html'\\w+);)");
        }

        public ExtendedHtmlUtility()
        {
        }

        private static string EntityLookup(string entity)
        {
            string str = "";
            string str1 = entity;
            string str2 = str1;
            if (str1 != null)
            {
                switch (str2)
                {
                    case "Aacute":
                        {
                            str = Convert.ToChar(193).ToString();
                            break;
                        }
                    case "aacute":
                        {
                            str = Convert.ToChar(225).ToString();
                            break;
                        }
                    case "acirc":
                        {
                            str = Convert.ToChar(226).ToString();
                            break;
                        }
                    case "Acirc":
                        {
                            str = Convert.ToChar(194).ToString();
                            break;
                        }
                    case "acute":
                        {
                            str = Convert.ToChar(180).ToString();
                            break;
                        }
                    case "AElig":
                        {
                            str = Convert.ToChar(198).ToString();
                            break;
                        }
                    case "aelig":
                        {
                            str = Convert.ToChar(230).ToString();
                            break;
                        }
                    case "Agrave":
                        {
                            str = Convert.ToChar(192).ToString();
                            break;
                        }
                    case "agrave":
                        {
                            str = Convert.ToChar(224).ToString();
                            break;
                        }
                    case "alefsym":
                        {
                            str = Convert.ToChar(8501).ToString();
                            break;
                        }
                    case "Alpha":
                        {
                            str = Convert.ToChar(913).ToString();
                            break;
                        }
                    case "alpha":
                        {
                            str = Convert.ToChar(945).ToString();
                            break;
                        }
                    case "amp":
                        {
                            str = Convert.ToChar(38).ToString();
                            break;
                        }
                    case "and":
                        {
                            str = Convert.ToChar(8743).ToString();
                            break;
                        }
                    case "ang":
                        {
                            str = Convert.ToChar(8736).ToString();
                            break;
                        }
                    case "aring":
                        {
                            str = Convert.ToChar(229).ToString();
                            break;
                        }
                    case "Aring":
                        {
                            str = Convert.ToChar(197).ToString();
                            break;
                        }
                    case "asymp":
                        {
                            str = Convert.ToChar(8776).ToString();
                            break;
                        }
                    case "Atilde":
                        {
                            str = Convert.ToChar(195).ToString();
                            break;
                        }
                    case "atilde":
                        {
                            str = Convert.ToChar(227).ToString();
                            break;
                        }
                    case "auml":
                        {
                            str = Convert.ToChar(228).ToString();
                            break;
                        }
                    case "Auml":
                        {
                            str = Convert.ToChar(196).ToString();
                            break;
                        }
                    case "bdquo":
                        {
                            str = Convert.ToChar(8222).ToString();
                            break;
                        }
                    case "Beta":
                        {
                            str = Convert.ToChar(914).ToString();
                            break;
                        }
                    case "beta":
                        {
                            str = Convert.ToChar(946).ToString();
                            break;
                        }
                    case "brvbar":
                        {
                            str = Convert.ToChar(166).ToString();
                            break;
                        }
                    case "bull":
                        {
                            str = Convert.ToChar(8226).ToString();
                            break;
                        }
                    case "cap":
                        {
                            str = Convert.ToChar(8745).ToString();
                            break;
                        }
                    case "Ccedil":
                        {
                            str = Convert.ToChar(199).ToString();
                            break;
                        }
                    case "ccedil":
                        {
                            str = Convert.ToChar(231).ToString();
                            break;
                        }
                    case "cedil":
                        {
                            str = Convert.ToChar(184).ToString();
                            break;
                        }
                    case "cent":
                        {
                            str = Convert.ToChar(162).ToString();
                            break;
                        }
                    case "chi":
                        {
                            str = Convert.ToChar(967).ToString();
                            break;
                        }
                    case "Chi":
                        {
                            str = Convert.ToChar(935).ToString();
                            break;
                        }
                    case "circ":
                        {
                            str = Convert.ToChar(710).ToString();
                            break;
                        }
                    case "clubs":
                        {
                            str = Convert.ToChar(9827).ToString();
                            break;
                        }
                    case "cong":
                        {
                            str = Convert.ToChar(8773).ToString();
                            break;
                        }
                    case "copy":
                        {
                            str = Convert.ToChar(169).ToString();
                            break;
                        }
                    case "crarr":
                        {
                            str = Convert.ToChar(8629).ToString();
                            break;
                        }
                    case "cup":
                        {
                            str = Convert.ToChar(8746).ToString();
                            break;
                        }
                    case "curren":
                        {
                            str = Convert.ToChar(164).ToString();
                            break;
                        }
                    case "dagger":
                        {
                            str = Convert.ToChar(8224).ToString();
                            break;
                        }
                    case "Dagger":
                        {
                            str = Convert.ToChar(8225).ToString();
                            break;
                        }
                    case "darr":
                        {
                            str = Convert.ToChar(8595).ToString();
                            break;
                        }
                    case "dArr":
                        {
                            str = Convert.ToChar(8659).ToString();
                            break;
                        }
                    case "deg":
                        {
                            str = Convert.ToChar(176).ToString();
                            break;
                        }
                    case "Delta":
                        {
                            str = Convert.ToChar(916).ToString();
                            break;
                        }
                    case "delta":
                        {
                            str = Convert.ToChar(948).ToString();
                            break;
                        }
                    case "diams":
                        {
                            str = Convert.ToChar(9830).ToString();
                            break;
                        }
                    case "divide":
                        {
                            str = Convert.ToChar(247).ToString();
                            break;
                        }
                    case "eacute":
                        {
                            str = Convert.ToChar(233).ToString();
                            break;
                        }
                    case "Eacute":
                        {
                            str = Convert.ToChar(201).ToString();
                            break;
                        }
                    case "Ecirc":
                        {
                            str = Convert.ToChar(202).ToString();
                            break;
                        }
                    case "ecirc":
                        {
                            str = Convert.ToChar(234).ToString();
                            break;
                        }
                    case "Egrave":
                        {
                            str = Convert.ToChar(200).ToString();
                            break;
                        }
                    case "egrave":
                        {
                            str = Convert.ToChar(232).ToString();
                            break;
                        }
                    case "empty":
                        {
                            str = Convert.ToChar(8709).ToString();
                            break;
                        }
                    case "emsp":
                        {
                            str = Convert.ToChar(8195).ToString();
                            break;
                        }
                    case "ensp":
                        {
                            str = Convert.ToChar(8194).ToString();
                            break;
                        }
                    case "epsilon":
                        {
                            str = Convert.ToChar(949).ToString();
                            break;
                        }
                    case "Epsilon":
                        {
                            str = Convert.ToChar(917).ToString();
                            break;
                        }
                    case "equiv":
                        {
                            str = Convert.ToChar(8801).ToString();
                            break;
                        }
                    case "Eta":
                        {
                            str = Convert.ToChar(919).ToString();
                            break;
                        }
                    case "eta":
                        {
                            str = Convert.ToChar(951).ToString();
                            break;
                        }
                    case "eth":
                        {
                            str = Convert.ToChar(240).ToString();
                            break;
                        }
                    case "ETH":
                        {
                            str = Convert.ToChar(208).ToString();
                            break;
                        }
                    case "Euml":
                        {
                            str = Convert.ToChar(203).ToString();
                            break;
                        }
                    case "euml":
                        {
                            str = Convert.ToChar(235).ToString();
                            break;
                        }
                    case "euro":
                        {
                            str = Convert.ToChar(8364).ToString();
                            break;
                        }
                    case "exist":
                        {
                            str = Convert.ToChar(8707).ToString();
                            break;
                        }
                    case "fnof":
                        {
                            str = Convert.ToChar(402).ToString();
                            break;
                        }
                    case "forall":
                        {
                            str = Convert.ToChar(8704).ToString();
                            break;
                        }
                    case "frac12":
                        {
                            str = Convert.ToChar(189).ToString();
                            break;
                        }
                    case "frac14":
                        {
                            str = Convert.ToChar(188).ToString();
                            break;
                        }
                    case "frac34":
                        {
                            str = Convert.ToChar(190).ToString();
                            break;
                        }
                    case "frasl":
                        {
                            str = Convert.ToChar(8260).ToString();
                            break;
                        }
                    case "gamma":
                        {
                            str = Convert.ToChar(947).ToString();
                            break;
                        }
                    case "Gamma":
                        {
                            str = Convert.ToChar(915).ToString();
                            break;
                        }
                    case "ge":
                        {
                            str = Convert.ToChar(8805).ToString();
                            break;
                        }
                    case "gt":
                        {
                            str = Convert.ToChar(62).ToString();
                            break;
                        }
                    case "hArr":
                        {
                            str = Convert.ToChar(8660).ToString();
                            break;
                        }
                    case "harr":
                        {
                            str = Convert.ToChar(8596).ToString();
                            break;
                        }
                    case "hearts":
                        {
                            str = Convert.ToChar(9829).ToString();
                            break;
                        }
                    case "hellip":
                        {
                            str = Convert.ToChar(8230).ToString();
                            break;
                        }
                    case "Iacute":
                        {
                            str = Convert.ToChar(205).ToString();
                            break;
                        }
                    case "iacute":
                        {
                            str = Convert.ToChar(237).ToString();
                            break;
                        }
                    case "icirc":
                        {
                            str = Convert.ToChar(238).ToString();
                            break;
                        }
                    case "Icirc":
                        {
                            str = Convert.ToChar(206).ToString();
                            break;
                        }
                    case "iexcl":
                        {
                            str = Convert.ToChar(161).ToString();
                            break;
                        }
                    case "Igrave":
                        {
                            str = Convert.ToChar(204).ToString();
                            break;
                        }
                    case "igrave":
                        {
                            str = Convert.ToChar(236).ToString();
                            break;
                        }
                    case "image":
                        {
                            str = Convert.ToChar(8465).ToString();
                            break;
                        }
                    case "infin":
                        {
                            str = Convert.ToChar(8734).ToString();
                            break;
                        }
                    case "int":
                        {
                            str = Convert.ToChar(8747).ToString();
                            break;
                        }
                    case "Iota":
                        {
                            str = Convert.ToChar(921).ToString();
                            break;
                        }
                    case "iota":
                        {
                            str = Convert.ToChar(953).ToString();
                            break;
                        }
                    case "iquest":
                        {
                            str = Convert.ToChar(191).ToString();
                            break;
                        }
                    case "isin":
                        {
                            str = Convert.ToChar(8712).ToString();
                            break;
                        }
                    case "iuml":
                        {
                            str = Convert.ToChar(239).ToString();
                            break;
                        }
                    case "Iuml":
                        {
                            str = Convert.ToChar(207).ToString();
                            break;
                        }
                    case "kappa":
                        {
                            str = Convert.ToChar(954).ToString();
                            break;
                        }
                    case "Kappa":
                        {
                            str = Convert.ToChar(922).ToString();
                            break;
                        }
                    case "Lambda":
                        {
                            str = Convert.ToChar(923).ToString();
                            break;
                        }
                    case "lambda":
                        {
                            str = Convert.ToChar(955).ToString();
                            break;
                        }
                    case "lang":
                        {
                            str = Convert.ToChar(9001).ToString();
                            break;
                        }
                    case "laquo":
                        {
                            str = Convert.ToChar(171).ToString();
                            break;
                        }
                    case "larr":
                        {
                            str = Convert.ToChar(8592).ToString();
                            break;
                        }
                    case "lArr":
                        {
                            str = Convert.ToChar(8656).ToString();
                            break;
                        }
                    case "lceil":
                        {
                            str = Convert.ToChar(8968).ToString();
                            break;
                        }
                    case "ldquo":
                        {
                            str = Convert.ToChar(8220).ToString();
                            break;
                        }
                    case "le":
                        {
                            str = Convert.ToChar(8804).ToString();
                            break;
                        }
                    case "lfloor":
                        {
                            str = Convert.ToChar(8970).ToString();
                            break;
                        }
                    case "lowast":
                        {
                            str = Convert.ToChar(8727).ToString();
                            break;
                        }
                    case "loz":
                        {
                            str = Convert.ToChar(9674).ToString();
                            break;
                        }
                    case "lrm":
                        {
                            str = Convert.ToChar(8206).ToString();
                            break;
                        }
                    case "lsaquo":
                        {
                            str = Convert.ToChar(8249).ToString();
                            break;
                        }
                    case "lsquo":
                        {
                            str = Convert.ToChar(8216).ToString();
                            break;
                        }
                    case "lt":
                        {
                            str = Convert.ToChar(60).ToString();
                            break;
                        }
                    case "macr":
                        {
                            str = Convert.ToChar(175).ToString();
                            break;
                        }
                    case "mdash":
                        {
                            str = Convert.ToChar(8212).ToString();
                            break;
                        }
                    case "micro":
                        {
                            str = Convert.ToChar(181).ToString();
                            break;
                        }
                    case "middot":
                        {
                            str = Convert.ToChar(183).ToString();
                            break;
                        }
                    case "minus":
                        {
                            str = Convert.ToChar(8722).ToString();
                            break;
                        }
                    case "Mu":
                        {
                            str = Convert.ToChar(924).ToString();
                            break;
                        }
                    case "mu":
                        {
                            str = Convert.ToChar(956).ToString();
                            break;
                        }
                    case "nabla":
                        {
                            str = Convert.ToChar(8711).ToString();
                            break;
                        }
                    case "nbsp":
                        {
                            str = Convert.ToChar(160).ToString();
                            break;
                        }
                    case "ndash":
                        {
                            str = Convert.ToChar(8211).ToString();
                            break;
                        }
                    case "ne":
                        {
                            str = Convert.ToChar(8800).ToString();
                            break;
                        }
                    case "ni":
                        {
                            str = Convert.ToChar(8715).ToString();
                            break;
                        }
                    case "not":
                        {
                            str = Convert.ToChar(172).ToString();
                            break;
                        }
                    case "notin":
                        {
                            str = Convert.ToChar(8713).ToString();
                            break;
                        }
                    case "nsub":
                        {
                            str = Convert.ToChar(8836).ToString();
                            break;
                        }
                    case "ntilde":
                        {
                            str = Convert.ToChar(241).ToString();
                            break;
                        }
                    case "Ntilde":
                        {
                            str = Convert.ToChar(209).ToString();
                            break;
                        }
                    case "Nu":
                        {
                            str = Convert.ToChar(925).ToString();
                            break;
                        }
                    case "nu":
                        {
                            str = Convert.ToChar(957).ToString();
                            break;
                        }
                    case "oacute":
                        {
                            str = Convert.ToChar(243).ToString();
                            break;
                        }
                    case "Oacute":
                        {
                            str = Convert.ToChar(211).ToString();
                            break;
                        }
                    case "Ocirc":
                        {
                            str = Convert.ToChar(212).ToString();
                            break;
                        }
                    case "ocirc":
                        {
                            str = Convert.ToChar(244).ToString();
                            break;
                        }
                    case "OElig":
                        {
                            str = Convert.ToChar(338).ToString();
                            break;
                        }
                    case "oelig":
                        {
                            str = Convert.ToChar(339).ToString();
                            break;
                        }
                    case "ograve":
                        {
                            str = Convert.ToChar(242).ToString();
                            break;
                        }
                    case "Ograve":
                        {
                            str = Convert.ToChar(210).ToString();
                            break;
                        }
                    case "oline":
                        {
                            str = Convert.ToChar(8254).ToString();
                            break;
                        }
                    case "Omega":
                        {
                            str = Convert.ToChar(937).ToString();
                            break;
                        }
                    case "omega":
                        {
                            str = Convert.ToChar(969).ToString();
                            break;
                        }
                    case "Omicron":
                        {
                            str = Convert.ToChar(927).ToString();
                            break;
                        }
                    case "omicron":
                        {
                            str = Convert.ToChar(959).ToString();
                            break;
                        }
                    case "oplus":
                        {
                            str = Convert.ToChar(8853).ToString();
                            break;
                        }
                    case "or":
                        {
                            str = Convert.ToChar(8744).ToString();
                            break;
                        }
                    case "ordf":
                        {
                            str = Convert.ToChar(170).ToString();
                            break;
                        }
                    case "ordm":
                        {
                            str = Convert.ToChar(186).ToString();
                            break;
                        }
                    case "Oslash":
                        {
                            str = Convert.ToChar(216).ToString();
                            break;
                        }
                    case "oslash":
                        {
                            str = Convert.ToChar(248).ToString();
                            break;
                        }
                    case "otilde":
                        {
                            str = Convert.ToChar(245).ToString();
                            break;
                        }
                    case "Otilde":
                        {
                            str = Convert.ToChar(213).ToString();
                            break;
                        }
                    case "otimes":
                        {
                            str = Convert.ToChar(8855).ToString();
                            break;
                        }
                    case "Ouml":
                        {
                            str = Convert.ToChar(214).ToString();
                            break;
                        }
                    case "ouml":
                        {
                            str = Convert.ToChar(246).ToString();
                            break;
                        }
                    case "para":
                        {
                            str = Convert.ToChar(182).ToString();
                            break;
                        }
                    case "part":
                        {
                            str = Convert.ToChar(8706).ToString();
                            break;
                        }
                    case "permil":
                        {
                            str = Convert.ToChar(8240).ToString();
                            break;
                        }
                    case "perp":
                        {
                            str = Convert.ToChar(8869).ToString();
                            break;
                        }
                    case "Phi":
                        {
                            str = Convert.ToChar(934).ToString();
                            break;
                        }
                    case "phi":
                        {
                            str = Convert.ToChar(966).ToString();
                            break;
                        }
                    case "Pi":
                        {
                            str = Convert.ToChar(928).ToString();
                            break;
                        }
                    case "pi":
                        {
                            str = Convert.ToChar(960).ToString();
                            break;
                        }
                    case "piv":
                        {
                            str = Convert.ToChar(982).ToString();
                            break;
                        }
                    case "plusmn":
                        {
                            str = Convert.ToChar(177).ToString();
                            break;
                        }
                    case "pound":
                        {
                            str = Convert.ToChar(163).ToString();
                            break;
                        }
                    case "Prime":
                        {
                            str = Convert.ToChar(8243).ToString();
                            break;
                        }
                    case "prime":
                        {
                            str = Convert.ToChar(8242).ToString();
                            break;
                        }
                    case "prod":
                        {
                            str = Convert.ToChar(8719).ToString();
                            break;
                        }
                    case "prop":
                        {
                            str = Convert.ToChar(8733).ToString();
                            break;
                        }
                    case "psi":
                        {
                            str = Convert.ToChar(968).ToString();
                            break;
                        }
                    case "Psi":
                        {
                            str = Convert.ToChar(936).ToString();
                            break;
                        }
                    case "quot":
                        {
                            str = Convert.ToChar(34).ToString();
                            break;
                        }
                    case "radic":
                        {
                            str = Convert.ToChar(8730).ToString();
                            break;
                        }
                    case "rang":
                        {
                            str = Convert.ToChar(9002).ToString();
                            break;
                        }
                    case "raquo":
                        {
                            str = Convert.ToChar(187).ToString();
                            break;
                        }
                    case "rarr":
                        {
                            str = Convert.ToChar(8594).ToString();
                            break;
                        }
                    case "rArr":
                        {
                            str = Convert.ToChar(8658).ToString();
                            break;
                        }
                    case "rceil":
                        {
                            str = Convert.ToChar(8969).ToString();
                            break;
                        }
                    case "rdquo":
                        {
                            str = Convert.ToChar(8221).ToString();
                            break;
                        }
                    case "real":
                        {
                            str = Convert.ToChar(8476).ToString();
                            break;
                        }
                    case "reg":
                        {
                            str = Convert.ToChar(174).ToString();
                            break;
                        }
                    case "rfloor":
                        {
                            str = Convert.ToChar(8971).ToString();
                            break;
                        }
                    case "rho":
                        {
                            str = Convert.ToChar(961).ToString();
                            break;
                        }
                    case "Rho":
                        {
                            str = Convert.ToChar(929).ToString();
                            break;
                        }
                    case "rlm":
                        {
                            str = Convert.ToChar(8207).ToString();
                            break;
                        }
                    case "rsaquo":
                        {
                            str = Convert.ToChar(8250).ToString();
                            break;
                        }
                    case "rsquo":
                        {
                            str = Convert.ToChar(8217).ToString();
                            break;
                        }
                    case "sbquo":
                        {
                            str = Convert.ToChar(8218).ToString();
                            break;
                        }
                    case "Scaron":
                        {
                            str = Convert.ToChar(352).ToString();
                            break;
                        }
                    case "scaron":
                        {
                            str = Convert.ToChar(353).ToString();
                            break;
                        }
                    case "sdot":
                        {
                            str = Convert.ToChar(8901).ToString();
                            break;
                        }
                    case "sect":
                        {
                            str = Convert.ToChar(167).ToString();
                            break;
                        }
                    case "shy":
                        {
                            str = Convert.ToChar(173).ToString();
                            break;
                        }
                    case "sigma":
                        {
                            str = Convert.ToChar(963).ToString();
                            break;
                        }
                    case "Sigma":
                        {
                            str = Convert.ToChar(931).ToString();
                            break;
                        }
                    case "sigmaf":
                        {
                            str = Convert.ToChar(962).ToString();
                            break;
                        }
                    case "sim":
                        {
                            str = Convert.ToChar(8764).ToString();
                            break;
                        }
                    case "spades":
                        {
                            str = Convert.ToChar(9824).ToString();
                            break;
                        }
                    case "sub":
                        {
                            str = Convert.ToChar(8834).ToString();
                            break;
                        }
                    case "sube":
                        {
                            str = Convert.ToChar(8838).ToString();
                            break;
                        }
                    case "sum":
                        {
                            str = Convert.ToChar(8721).ToString();
                            break;
                        }
                    case "sup":
                        {
                            str = Convert.ToChar(8835).ToString();
                            break;
                        }
                    case "sup1":
                        {
                            str = Convert.ToChar(185).ToString();
                            break;
                        }
                    case "sup2":
                        {
                            str = Convert.ToChar(178).ToString();
                            break;
                        }
                    case "sup3":
                        {
                            str = Convert.ToChar(179).ToString();
                            break;
                        }
                    case "supe":
                        {
                            str = Convert.ToChar(8839).ToString();
                            break;
                        }
                    case "szlig":
                        {
                            str = Convert.ToChar(223).ToString();
                            break;
                        }
                    case "Tau":
                        {
                            str = Convert.ToChar(932).ToString();
                            break;
                        }
                    case "tau":
                        {
                            str = Convert.ToChar(964).ToString();
                            break;
                        }
                    case "there4":
                        {
                            str = Convert.ToChar(8756).ToString();
                            break;
                        }
                    case "theta":
                        {
                            str = Convert.ToChar(952).ToString();
                            break;
                        }
                    case "Theta":
                        {
                            str = Convert.ToChar(920).ToString();
                            break;
                        }
                    case "thetasym":
                        {
                            str = Convert.ToChar(977).ToString();
                            break;
                        }
                    case "thinsp":
                        {
                            str = Convert.ToChar(8201).ToString();
                            break;
                        }
                    case "thorn":
                        {
                            str = Convert.ToChar(254).ToString();
                            break;
                        }
                    case "THORN":
                        {
                            str = Convert.ToChar(222).ToString();
                            break;
                        }
                    case "tilde":
                        {
                            str = Convert.ToChar(732).ToString();
                            break;
                        }
                    case "times":
                        {
                            str = Convert.ToChar(215).ToString();
                            break;
                        }
                    case "trade":
                        {
                            str = Convert.ToChar(8482).ToString();
                            break;
                        }
                    case "Uacute":
                        {
                            str = Convert.ToChar(218).ToString();
                            break;
                        }
                    case "uacute":
                        {
                            str = Convert.ToChar(250).ToString();
                            break;
                        }
                    case "uarr":
                        {
                            str = Convert.ToChar(8593).ToString();
                            break;
                        }
                    case "uArr":
                        {
                            str = Convert.ToChar(8657).ToString();
                            break;
                        }
                    case "Ucirc":
                        {
                            str = Convert.ToChar(219).ToString();
                            break;
                        }
                    case "ucirc":
                        {
                            str = Convert.ToChar(251).ToString();
                            break;
                        }
                    case "Ugrave":
                        {
                            str = Convert.ToChar(217).ToString();
                            break;
                        }
                    case "ugrave":
                        {
                            str = Convert.ToChar(249).ToString();
                            break;
                        }
                    case "uml":
                        {
                            str = Convert.ToChar(168).ToString();
                            break;
                        }
                    case "upsih":
                        {
                            str = Convert.ToChar(978).ToString();
                            break;
                        }
                    case "Upsilon":
                        {
                            str = Convert.ToChar(933).ToString();
                            break;
                        }
                    case "upsilon":
                        {
                            str = Convert.ToChar(965).ToString();
                            break;
                        }
                    case "Uuml":
                        {
                            str = Convert.ToChar(220).ToString();
                            break;
                        }
                    case "uuml":
                        {
                            str = Convert.ToChar(252).ToString();
                            break;
                        }
                    case "weierp":
                        {
                            str = Convert.ToChar(8472).ToString();
                            break;
                        }
                    case "Xi":
                        {
                            str = Convert.ToChar(926).ToString();
                            break;
                        }
                    case "xi":
                        {
                            str = Convert.ToChar(958).ToString();
                            break;
                        }
                    case "yacute":
                        {
                            str = Convert.ToChar(253).ToString();
                            break;
                        }
                    case "Yacute":
                        {
                            str = Convert.ToChar(221).ToString();
                            break;
                        }
                    case "yen":
                        {
                            str = Convert.ToChar(165).ToString();
                            break;
                        }
                    case "Yuml":
                        {
                            str = Convert.ToChar(376).ToString();
                            break;
                        }
                    case "yuml":
                        {
                            str = Convert.ToChar(255).ToString();
                            break;
                        }
                    case "zeta":
                        {
                            str = Convert.ToChar(950).ToString();
                            break;
                        }
                    case "Zeta":
                        {
                            str = Convert.ToChar(918).ToString();
                            break;
                        }
                    case "zwj":
                        {
                            str = Convert.ToChar(8205).ToString();
                            break;
                        }
                    case "zwnj":
                        {
                            str = Convert.ToChar(8204).ToString();
                            break;
                        }
                }
            }
            return str;
        }

        public static int HexToInt(string hexstr)
        {
            int num = 0;
            hexstr = hexstr.ToUpper();
            char[] charArray = hexstr.ToCharArray();
            for (int i = (int)charArray.Length - 1; i >= 0; i--)
            {
                if (charArray[i] >= '0' && charArray[i] <= '9')
                {
                    num = num + (charArray[i] - 48) * (char)((int)Math.Pow(16, (double)((int)charArray.Length - 1 - i)));
                }
                else if (charArray[i] < 'A' || charArray[i] > 'F')
                {
                    num = 0;
                    break;
                }
                else
                {
                    num = num + (charArray[i] - 55) * (char)((int)Math.Pow(16, (double)((int)charArray.Length - 1 - i)));
                }
            }
            return num;
        }

        public static string HtmlEntityDecode(string encodedText)
        {
            return ExtendedHtmlUtility.entityResolver.Replace(encodedText, new MatchEvaluator(ExtendedHtmlUtility.ResolveEntityAngleAmp));
        }

        public static string HtmlEntityDecode(string encodedText, bool encodeTagsToo)
        {
            if (encodeTagsToo)
            {
                return ExtendedHtmlUtility.entityResolver.Replace(encodedText, new MatchEvaluator(ExtendedHtmlUtility.ResolveEntityAngleAmp));
            }
            return ExtendedHtmlUtility.entityResolver.Replace(encodedText, new MatchEvaluator(ExtendedHtmlUtility.ResolveEntityNotAngleAmp));
        }

        public static string HtmlEntityEncode(string unicodeText)
        {
            return ExtendedHtmlUtility.HtmlEntityEncode(unicodeText, true);
        }

        public static string HtmlEntityEncode(string unicodeText, bool encodeTagsToo)
        {
            string empty = string.Empty;
            string str = unicodeText;
            for (int i = 0; i < str.Length; i++)
            {
                char chr = str[i];
                int num = chr;
                int num1 = num;
                if (num1 != 38)
                {
                    switch (num1)
                    {
                        case 60:
                            {
                                if (!encodeTagsToo)
                                {
                                    break;
                                }
                                empty = string.Concat(empty, "&lt;");
                                break;
                            }
                        case 61:
                            {
                                if (chr < ' ' || chr > '~')
                                {
                                    empty = string.Concat(empty, string.Concat("&#", num.ToString(NumberFormatInfo.InvariantInfo), ";"));
                                    break;
                                }
                                else
                                {
                                    empty = string.Concat(empty, chr);
                                    break;
                                }
                            }
                        case 62:
                            {
                                if (!encodeTagsToo)
                                {
                                    break;
                                }
                                empty = string.Concat(empty, "&gt;");
                                break;
                            }
                        default:
                            {
                                goto case 61;
                            }
                    }
                }
                else if (encodeTagsToo)
                {
                    empty = string.Concat(empty, "&amp;");
                }
            }
            return empty;
        }

        private static string ResolveEntityAngleAmp(Match matchToProcess)
        {
            string str = "";
            if (matchToProcess.Groups["decimal"].Success)
            {
                char chr = Convert.ToChar(Convert.ToInt32(matchToProcess.Groups["decimal"].Value));
                str = chr.ToString();
            }
            else if (!matchToProcess.Groups["hex"].Success)
            {
                str = (!matchToProcess.Groups["html"].Success ? "Y" : ExtendedHtmlUtility.EntityLookup(matchToProcess.Groups["html"].Value));
            }
            else
            {
                char chr1 = Convert.ToChar(ExtendedHtmlUtility.HexToInt(matchToProcess.Groups["hex"].Value));
                str = chr1.ToString();
            }
            return str;
        }

        private static string ResolveEntityNotAngleAmp(Match matchToProcess)
        {
            string str = "";
            if (matchToProcess.Groups["decimal"].Success)
            {
                char chr = Convert.ToChar(Convert.ToInt32(matchToProcess.Groups["decimal"].Value));
                str = chr.ToString();
            }
            else if (matchToProcess.Groups["hex"].Success)
            {
                char chr1 = Convert.ToChar(ExtendedHtmlUtility.HexToInt(matchToProcess.Groups["hex"].Value));
                str = chr1.ToString();
            }
            else if (!matchToProcess.Groups["html"].Success)
            {
                str = "X";
            }
            else
            {
                string value = matchToProcess.Groups["html"].Value;
                string lower = value.ToLower();
                string str1 = lower;
                str = (lower == null || !(str1 == "lt") && !(str1 == "gt") && !(str1 == "amp") ? ExtendedHtmlUtility.EntityLookup(value) : string.Concat("&", value, ";"));
            }
            return str;
        }
    }
}
