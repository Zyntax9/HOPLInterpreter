﻿using HOPL.Interpreter.Errors;
using HOPL.Interpreter.Errors.TypeCheck;
using System;
using System.Collections.Generic;
using System.Text;
using static InternalTest.HoplScriptTests;

namespace InternalTest
{
    public struct ErroneousFile
    {
        public string File { get; set; }
        public string[] ImportPaths { get; set; }
        public string ErrorType { get; set; }
        public ExpectedError[] ExpectedErrors { get; set; }
    }

    public struct ExpectedError
    {
        public int ErrorCode { get; set; }
        public int Line { get; set; }
    }

    public static class ErrorMapping
    {
        public static ErroneousFile[] PreprocessingErrorFiles { get; set; } =
        {
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/OperationTypeMismatch.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 32, Line = 5 },
                    new ExpectedError() { ErrorCode = 32, Line = 10 },
                    new ExpectedError() { ErrorCode = 32, Line = 15 },
                    new ExpectedError() { ErrorCode = 32, Line = 20 },
                    new ExpectedError() { ErrorCode = 32, Line = 25 },
                    new ExpectedError() { ErrorCode = 32, Line = 30 },
                    new ExpectedError() { ErrorCode = 32, Line = 35 },
                    new ExpectedError() { ErrorCode = 32, Line = 40 },
                    new ExpectedError() { ErrorCode = 32, Line = 45 },
                    new ExpectedError() { ErrorCode = 32, Line = 50 },
                    new ExpectedError() { ErrorCode = 32, Line = 55 },
                    new ExpectedError() { ErrorCode = 32, Line = 60 },
                    new ExpectedError() { ErrorCode = 32, Line = 65 },
                    new ExpectedError() { ErrorCode = 32, Line = 70 },
                    new ExpectedError() { ErrorCode = 32, Line = 75 },
                    new ExpectedError() { ErrorCode = 32, Line = 80 },
                    new ExpectedError() { ErrorCode = 1, Line = 85 },
                    new ExpectedError() { ErrorCode = 1, Line = 90 },
                    new ExpectedError() { ErrorCode = 1, Line = 95 },
                    new ExpectedError() { ErrorCode = 1, Line = 100 },
                    new ExpectedError() { ErrorCode = 1, Line = 105 },
                    new ExpectedError() { ErrorCode = 1, Line = 110 },
                    new ExpectedError() { ErrorCode = 1, Line = 115 },
                    new ExpectedError() { ErrorCode = 1, Line = 120 },
                    new ExpectedError() { ErrorCode = 1, Line = 125 },
                    new ExpectedError() { ErrorCode = 1, Line = 130 },
                    new ExpectedError() { ErrorCode = 1, Line = 135 },
                    new ExpectedError() { ErrorCode = 1, Line = 140 },
                    new ExpectedError() { ErrorCode = 1, Line = 145 },
                    new ExpectedError() { ErrorCode = 1, Line = 150 },
                    new ExpectedError() { ErrorCode = 1, Line = 155 },
                    new ExpectedError() { ErrorCode = 1, Line = 160 },
                    new ExpectedError() { ErrorCode = 1, Line = 165 },
                    new ExpectedError() { ErrorCode = 1, Line = 170 },
                    new ExpectedError() { ErrorCode = 1, Line = 175 },
                    new ExpectedError() { ErrorCode = 1, Line = 180 },
                    new ExpectedError() { ErrorCode = 1, Line = 185 },
                    new ExpectedError() { ErrorCode = 1, Line = 190 },
                    new ExpectedError() { ErrorCode = 1, Line = 195 },
                    new ExpectedError() { ErrorCode = 1, Line = 200 },
                    new ExpectedError() { ErrorCode = 1, Line = 205 },
                    new ExpectedError() { ErrorCode = 1, Line = 210 },
                    new ExpectedError() { ErrorCode = 1, Line = 215 },
                    new ExpectedError() { ErrorCode = 1, Line = 220 },
                    new ExpectedError() { ErrorCode = 1, Line = 225 },
                    new ExpectedError() { ErrorCode = 1, Line = 230 },
                    new ExpectedError() { ErrorCode = 1, Line = 235 },
                    new ExpectedError() { ErrorCode = 1, Line = 240 },
                    new ExpectedError() { ErrorCode = 1, Line = 245 },
                    new ExpectedError() { ErrorCode = 1, Line = 250 },
                    new ExpectedError() { ErrorCode = 1, Line = 255 },
                    new ExpectedError() { ErrorCode = 1, Line = 260 },
                    new ExpectedError() { ErrorCode = 1, Line = 265 },
                    new ExpectedError() { ErrorCode = 1, Line = 270 },
                    new ExpectedError() { ErrorCode = 1, Line = 275 },
                    new ExpectedError() { ErrorCode = 1, Line = 280 },
                    new ExpectedError() { ErrorCode = 1, Line = 285 },
                    new ExpectedError() { ErrorCode = 1, Line = 290 },
                    new ExpectedError() { ErrorCode = 1, Line = 295 },
                    new ExpectedError() { ErrorCode = 1, Line = 300 },
                    new ExpectedError() { ErrorCode = 1, Line = 305 },
                    new ExpectedError() { ErrorCode = 1, Line = 310 },
                    new ExpectedError() { ErrorCode = 1, Line = 315 },
                    new ExpectedError() { ErrorCode = 1, Line = 320 },
                    new ExpectedError() { ErrorCode = 1, Line = 325 },
                    new ExpectedError() { ErrorCode = 1, Line = 330 },
                    new ExpectedError() { ErrorCode = 1, Line = 335 },
                    new ExpectedError() { ErrorCode = 1, Line = 340 },
                    new ExpectedError() { ErrorCode = 1, Line = 345 },
                    new ExpectedError() { ErrorCode = 1, Line = 350 },
                    new ExpectedError() { ErrorCode = 32, Line = 355 },
                    new ExpectedError() { ErrorCode = 2, Line = 360 },
                    new ExpectedError() { ErrorCode = 2, Line = 365 },
                    new ExpectedError() { ErrorCode = 2, Line = 370 },
                    new ExpectedError() { ErrorCode = 2, Line = 375 },
                    new ExpectedError() { ErrorCode = 2, Line = 380 },
                    new ExpectedError() { ErrorCode = 2, Line = 385 },
                    new ExpectedError() { ErrorCode = 17, Line = 390 },
                    new ExpectedError() { ErrorCode = 2, Line = 395 },
                    new ExpectedError() { ErrorCode = 2, Line = 400 },
                    new ExpectedError() { ErrorCode = 2, Line = 405 },
                    new ExpectedError() { ErrorCode = 2, Line = 410 },
                    new ExpectedError() { ErrorCode = 2, Line = 415 },
                    new ExpectedError() { ErrorCode = 2, Line = 420 },
                    new ExpectedError() { ErrorCode = 17, Line = 425 },
                    new ExpectedError() { ErrorCode = 2, Line = 430 },
                    new ExpectedError() { ErrorCode = 2, Line = 435 },
                    new ExpectedError() { ErrorCode = 2, Line = 440 },
                    new ExpectedError() { ErrorCode = 2, Line = 445 },
                    new ExpectedError() { ErrorCode = 2, Line = 450 },
                    new ExpectedError() { ErrorCode = 2, Line = 455 },
                    new ExpectedError() { ErrorCode = 17, Line = 460 },
                    new ExpectedError() { ErrorCode = 3, Line = 465 },
                    new ExpectedError() { ErrorCode = 3, Line = 470 },
                    new ExpectedError() { ErrorCode = 3, Line = 475 },
                    new ExpectedError() { ErrorCode = 3, Line = 480 },
                    new ExpectedError() { ErrorCode = 3, Line = 485 },
                    new ExpectedError() { ErrorCode = 3, Line = 490 },
                    new ExpectedError() { ErrorCode = 3, Line = 495 },
                    new ExpectedError() { ErrorCode = 3, Line = 500 },
                    new ExpectedError() { ErrorCode = 3, Line = 505 },
                    new ExpectedError() { ErrorCode = 3, Line = 510 },
                    new ExpectedError() { ErrorCode = 3, Line = 515 },
                    new ExpectedError() { ErrorCode = 3, Line = 520 },
                    new ExpectedError() { ErrorCode = 3, Line = 525 },
                    new ExpectedError() { ErrorCode = 3, Line = 530 },
                    new ExpectedError() { ErrorCode = 3, Line = 535 },
                    new ExpectedError() { ErrorCode = 3, Line = 540 },
                    new ExpectedError() { ErrorCode = 3, Line = 545 },
                    new ExpectedError() { ErrorCode = 3, Line = 550 },
                    new ExpectedError() { ErrorCode = 3, Line = 555 },
                    new ExpectedError() { ErrorCode = 3, Line = 560 },
                    new ExpectedError() { ErrorCode = 3, Line = 565 },
                    new ExpectedError() { ErrorCode = 3, Line = 570 },
                    new ExpectedError() { ErrorCode = 3, Line = 575 },
                    new ExpectedError() { ErrorCode = 3, Line = 580 },
                    new ExpectedError() { ErrorCode = 11, Line = 585 },
                    new ExpectedError() { ErrorCode = 11, Line = 590 },
                    new ExpectedError() { ErrorCode = 11, Line = 595 },
                    new ExpectedError() { ErrorCode = 11, Line = 600 },
                    new ExpectedError() { ErrorCode = 11, Line = 605 },
                    new ExpectedError() { ErrorCode = 11, Line = 610 },
                    new ExpectedError() { ErrorCode = 11, Line = 615 },
                    new ExpectedError() { ErrorCode = 11, Line = 620 },
                    new ExpectedError() { ErrorCode = 11, Line = 625 },
                    new ExpectedError() { ErrorCode = 11, Line = 630 },
                    new ExpectedError() { ErrorCode = 11, Line = 635 },
                    new ExpectedError() { ErrorCode = 11, Line = 640 },
                    new ExpectedError() { ErrorCode = 11, Line = 645 },
                    new ExpectedError() { ErrorCode = 11, Line = 650 },
                    new ExpectedError() { ErrorCode = 11, Line = 655 },
                    new ExpectedError() { ErrorCode = 11, Line = 660 },
                    new ExpectedError() { ErrorCode = 11, Line = 665 },
                    new ExpectedError() { ErrorCode = 11, Line = 670 },
                    new ExpectedError() { ErrorCode = 11, Line = 675 },
                    new ExpectedError() { ErrorCode = 11, Line = 680 },
                    new ExpectedError() { ErrorCode = 11, Line = 685 },
                    new ExpectedError() { ErrorCode = 11, Line = 690 },
                    new ExpectedError() { ErrorCode = 11, Line = 695 },
                    new ExpectedError() { ErrorCode = 11, Line = 700 },
                    new ExpectedError() { ErrorCode = 11, Line = 705 },
                    new ExpectedError() { ErrorCode = 11, Line = 710 },
                    new ExpectedError() { ErrorCode = 11, Line = 715 },
                    new ExpectedError() { ErrorCode = 11, Line = 720 },
                    new ExpectedError() { ErrorCode = 11, Line = 725 },
                    new ExpectedError() { ErrorCode = 11, Line = 730 },
                    new ExpectedError() { ErrorCode = 11, Line = 735 },
                    new ExpectedError() { ErrorCode = 11, Line = 740 },
                    new ExpectedError() { ErrorCode = 11, Line = 745 },
                    new ExpectedError() { ErrorCode = 11, Line = 750 },
                    new ExpectedError() { ErrorCode = 11, Line = 755 },
                    new ExpectedError() { ErrorCode = 11, Line = 760 },
                    new ExpectedError() { ErrorCode = 16, Line = 765 },
                    new ExpectedError() { ErrorCode = 16, Line = 770 },
                    new ExpectedError() { ErrorCode = 34, Line = 775 },
                    new ExpectedError() { ErrorCode = 34, Line = 780 },
                    new ExpectedError() { ErrorCode = 34, Line = 785 },
                    new ExpectedError() { ErrorCode = 34, Line = 790 },
                    new ExpectedError() { ErrorCode = 34, Line = 795 },
                    new ExpectedError() { ErrorCode = 34, Line = 800 },
                    new ExpectedError() { ErrorCode = 34, Line = 805 },
                    new ExpectedError() { ErrorCode = 34, Line = 810 },
                    new ExpectedError() { ErrorCode = 34, Line = 815 },
                    new ExpectedError() { ErrorCode = 34, Line = 820 },
                    new ExpectedError() { ErrorCode = 34, Line = 825 },
                    new ExpectedError() { ErrorCode = 33, Line = 830 },
                    new ExpectedError() { ErrorCode = 33, Line = 835 },
                    new ExpectedError() { ErrorCode = 33, Line = 840 },
                    new ExpectedError() { ErrorCode = 33, Line = 845 },
                    new ExpectedError() { ErrorCode = 33, Line = 850 },
                    new ExpectedError() { ErrorCode = 33, Line = 855 },
                    new ExpectedError() { ErrorCode = 33, Line = 860 },
                    new ExpectedError() { ErrorCode = 33, Line = 865 },
                    new ExpectedError() { ErrorCode = 33, Line = 870 },
                    new ExpectedError() { ErrorCode = 33, Line = 875 },
                    new ExpectedError() { ErrorCode = 33, Line = 880 },
                    new ExpectedError() { ErrorCode = 35, Line = 885 },
                    new ExpectedError() { ErrorCode = 35, Line = 890 },
                    new ExpectedError() { ErrorCode = 35, Line = 895 },
                    new ExpectedError() { ErrorCode = 35, Line = 900 },
                    new ExpectedError() { ErrorCode = 35, Line = 905 },
                    new ExpectedError() { ErrorCode = 35, Line = 910 },
                    new ExpectedError() { ErrorCode = 35, Line = 915 },
                    new ExpectedError() { ErrorCode = 35, Line = 920 },
                    new ExpectedError() { ErrorCode = 35, Line = 925 },
                    new ExpectedError() { ErrorCode = 35, Line = 930 }
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/Arguments.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 4, Line = 10 },
                    new ExpectedError() { ErrorCode = 4, Line = 15 },
                    new ExpectedError() { ErrorCode = 4, Line = 20 },
                    new ExpectedError() { ErrorCode = 4, Line = 25 },
                    new ExpectedError() { ErrorCode = 4, Line = 30 },
                    new ExpectedError() { ErrorCode = 4, Line = 35 },
                    new ExpectedError() { ErrorCode = 4, Line = 40 },
                    new ExpectedError() { ErrorCode = 4, Line = 47 },
                    new ExpectedError() { ErrorCode = 4, Line = 53 },
                    new ExpectedError() { ErrorCode = 4, Line = 59 },
                    new ExpectedError() { ErrorCode = 4, Line = 65 },
                    new ExpectedError() { ErrorCode = 4, Line = 71 },
                    new ExpectedError() { ErrorCode = 4, Line = 77 },
                    new ExpectedError() { ErrorCode = 4, Line = 83 },
                    new ExpectedError() { ErrorCode = 4, Line = 87 },
                    new ExpectedError() { ErrorCode = 4, Line = 89 },
                    new ExpectedError() { ErrorCode = 4, Line = 91 },
                    new ExpectedError() { ErrorCode = 4, Line = 93 },
                    new ExpectedError() { ErrorCode = 4, Line = 95 },
                    new ExpectedError() { ErrorCode = 4, Line = 97 },
                    new ExpectedError() { ErrorCode = 4, Line = 99 },
                    new ExpectedError() { ErrorCode = 31, Line = 101 },
                    new ExpectedError() { ErrorCode = 31, Line = 103 },
                    new ExpectedError() { ErrorCode = 31, Line = 105 },
                    new ExpectedError() { ErrorCode = 31, Line = 107 },
                    new ExpectedError() { ErrorCode = 31, Line = 109 },
                    new ExpectedError() { ErrorCode = 31, Line = 111 },
                    new ExpectedError() { ErrorCode = 31, Line = 113 },
                    new ExpectedError() { ErrorCode = 31, Line = 115 },
                    new ExpectedError() { ErrorCode = 31, Line = 117 },
                    new ExpectedError() { ErrorCode = 31, Line = 119 },
                    new ExpectedError() { ErrorCode = 31, Line = 121 },
                    new ExpectedError() { ErrorCode = 31, Line = 123 },
                    new ExpectedError() { ErrorCode = 31, Line = 125 },
                    new ExpectedError() { ErrorCode = 31, Line = 127 },
                    new ExpectedError() { ErrorCode = 31, Line = 129 },
                    new ExpectedError() { ErrorCode = 31, Line = 131 },
                    new ExpectedError() { ErrorCode = 31, Line = 150 },
                    new ExpectedError() { ErrorCode = 31, Line = 152 },
                    new ExpectedError() { ErrorCode = 31, Line = 154 },
                    new ExpectedError() { ErrorCode = 31, Line = 156 },
                    new ExpectedError() { ErrorCode = 31, Line = 158 },
                    new ExpectedError() { ErrorCode = 31, Line = 160 },
                    new ExpectedError() { ErrorCode = 31, Line = 162 },
                    new ExpectedError() { ErrorCode = 31, Line = 164 },
                    new ExpectedError() { ErrorCode = 31, Line = 166 },
                    new ExpectedError() { ErrorCode = 31, Line = 168 },
                    new ExpectedError() { ErrorCode = 31, Line = 170 },
                    new ExpectedError() { ErrorCode = 31, Line = 172 },
                    new ExpectedError() { ErrorCode = 31, Line = 174 },
                    new ExpectedError() { ErrorCode = 31, Line = 176 },
                    new ExpectedError() { ErrorCode = 31, Line = 178 },
                    new ExpectedError() { ErrorCode = 31, Line = 180 },
                    new ExpectedError() { ErrorCode = 8, Line = 187 },
                    new ExpectedError() { ErrorCode = 8, Line = 189 },
                    new ExpectedError() { ErrorCode = 8, Line = 191 },
                    new ExpectedError() { ErrorCode = 8, Line = 193 },
                    new ExpectedError() { ErrorCode = 8, Line = 195 },
                    new ExpectedError() { ErrorCode = 8, Line = 197 },
                    new ExpectedError() { ErrorCode = 8, Line = 199 },
                    new ExpectedError() { ErrorCode = 8, Line = 201 },
                    new ExpectedError() { ErrorCode = 8, Line = 203 },
                    new ExpectedError() { ErrorCode = 8, Line = 205 },
                    new ExpectedError() { ErrorCode = 8, Line = 207 },
                    new ExpectedError() { ErrorCode = 8, Line = 216 },
                    new ExpectedError() { ErrorCode = 8, Line = 222 },
                    new ExpectedError() { ErrorCode = 8, Line = 228 },
                    new ExpectedError() { ErrorCode = 8, Line = 234 },
                    new ExpectedError() { ErrorCode = 8, Line = 240 },
                    new ExpectedError() { ErrorCode = 8, Line = 246 },
                    new ExpectedError() { ErrorCode = 8, Line = 252 },
                    new ExpectedError() { ErrorCode = 8, Line = 258 },
                    new ExpectedError() { ErrorCode = 8, Line = 264 },
                    new ExpectedError() { ErrorCode = 8, Line = 270 },
                    new ExpectedError() { ErrorCode = 8, Line = 276 },
                    new ExpectedError() { ErrorCode = 13, Line = 280 },
                    new ExpectedError() { ErrorCode = 13, Line = 282 },
                    new ExpectedError() { ErrorCode = 13, Line = 284 },
                    new ExpectedError() { ErrorCode = 13, Line = 286 },
                    new ExpectedError() { ErrorCode = 13, Line = 288 },
                    new ExpectedError() { ErrorCode = 13, Line = 290 },
                    new ExpectedError() { ErrorCode = 13, Line = 292 },
                    new ExpectedError() { ErrorCode = 13, Line = 294 },
                    new ExpectedError() { ErrorCode = 13, Line = 296 },
                    new ExpectedError() { ErrorCode = 13, Line = 298 },
                    new ExpectedError() { ErrorCode = 13, Line = 300 }
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/Undeclared.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 14, Line = 5 },
                    new ExpectedError() { ErrorCode = 14, Line = 10 },
                    new ExpectedError() { ErrorCode = 14, Line = 15 },
                    new ExpectedError() { ErrorCode = 14, Line = 20 },
                    new ExpectedError() { ErrorCode = 5, Line = 25 },
                    new ExpectedError() { ErrorCode = 5, Line = 30 },
                    new ExpectedError() { ErrorCode = 5, Line = 35 },
                    new ExpectedError() { ErrorCode = 5, Line = 40 },
                    new ExpectedError() { ErrorCode = 10, Line = 46 },
                    new ExpectedError() { ErrorCode = 10, Line = 53 },
                    new ExpectedError() { ErrorCode = 10, Line = 60 },
                    new ExpectedError() { ErrorCode = 10, Line = 67 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/Indexing.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 26, Line = 5 },
                    new ExpectedError() { ErrorCode = 26, Line = 10 },
                    new ExpectedError() { ErrorCode = 26, Line = 15 },
                    new ExpectedError() { ErrorCode = 26, Line = 20 },
                    new ExpectedError() { ErrorCode = 27, Line = 25 },
                    new ExpectedError() { ErrorCode = 28, Line = 32 },
                    new ExpectedError() { ErrorCode = 28, Line = 38 },
                    new ExpectedError() { ErrorCode = 28, Line = 43 },
                    new ExpectedError() { ErrorCode = 28, Line = 49 },
                    new ExpectedError() { ErrorCode = 28, Line = 54 },
                    new ExpectedError() { ErrorCode = 28, Line = 60 },
                    new ExpectedError() { ErrorCode = 29, Line = 68 },
                    new ExpectedError() { ErrorCode = 29, Line = 73 },
                    new ExpectedError() { ErrorCode = 29, Line = 79 },
                    new ExpectedError() { ErrorCode = 29, Line = 84 },
                    new ExpectedError() { ErrorCode = 29, Line = 90 },
                    new ExpectedError() { ErrorCode = 29, Line = 95 },
                    new ExpectedError() { ErrorCode = 29, Line = 101 },
                    new ExpectedError() { ErrorCode = 30, Line = 106 },
                    new ExpectedError() { ErrorCode = 30, Line = 111 },
                    new ExpectedError() { ErrorCode = 30, Line = 116 },
                    new ExpectedError() { ErrorCode = 30, Line = 121 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/Callables.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 18, Line = 7 },
                    new ExpectedError() { ErrorCode = 18, Line = 13 },
                    new ExpectedError() { ErrorCode = 18, Line = 18 },
                    new ExpectedError() { ErrorCode = 18, Line = 24 },
                    new ExpectedError() { ErrorCode = 18, Line = 29 },
                    new ExpectedError() { ErrorCode = 18, Line = 35 },
                    new ExpectedError() { ErrorCode = 18, Line = 40 },
                    new ExpectedError() { ErrorCode = 18, Line = 46 },
                    new ExpectedError() { ErrorCode = 12, Line = 54 },
                    new ExpectedError() { ErrorCode = 12, Line = 56 },
                    new ExpectedError() { ErrorCode = 12, Line = 58 },
                    new ExpectedError() { ErrorCode = 12, Line = 60 },
                    new ExpectedError() { ErrorCode = 19, Line = 62 },
                    new ExpectedError() { ErrorCode = 19, Line = 64 },
                    new ExpectedError() { ErrorCode = 19, Line = 66 },
                    new ExpectedError() { ErrorCode = 19, Line = 68 },
                    new ExpectedError() { ErrorCode = 19, Line = 70 },
                    new ExpectedError() { ErrorCode = 19, Line = 72 },
                    new ExpectedError() { ErrorCode = 19, Line = 74 },
                    new ExpectedError() { ErrorCode = 19, Line = 76 },
                    new ExpectedError() { ErrorCode = 19, Line = 78 },
                    new ExpectedError() { ErrorCode = 19, Line = 80 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/Unpacking.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 37, Line = 12 },
                    new ExpectedError() { ErrorCode = 37, Line = 18 },
                    new ExpectedError() { ErrorCode = 37, Line = 24 },
                    new ExpectedError() { ErrorCode = 37, Line = 30 },
                    new ExpectedError() { ErrorCode = 37, Line = 36 },
                    new ExpectedError() { ErrorCode = 38, Line = 42 },
                    new ExpectedError() { ErrorCode = 38, Line = 48 },
                    new ExpectedError() { ErrorCode = 38, Line = 54 },
                    new ExpectedError() { ErrorCode = 39, Line = 60 },
                    new ExpectedError() { ErrorCode = 39, Line = 66 },
                    new ExpectedError() { ErrorCode = 39, Line = 72 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/Variables.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 40, Line = 10 },
                    new ExpectedError() { ErrorCode = 40, Line = 16 },
                    new ExpectedError() { ErrorCode = 40, Line = 22 },
                    new ExpectedError() { ErrorCode = 40, Line = 28 },
                    new ExpectedError() { ErrorCode = 20, Line = 35 },
                    new ExpectedError() { ErrorCode = 20, Line = 42 },
                    new ExpectedError() { ErrorCode = 20, Line = 49 },
                    new ExpectedError() { ErrorCode = 20, Line = 56 },
                    new ExpectedError() { ErrorCode = 20, Line = 63 },
                    new ExpectedError() { ErrorCode = 20, Line = 70 },
                    new ExpectedError() { ErrorCode = 20, Line = 77 },
                    new ExpectedError() { ErrorCode = 20, Line = 84 },
                    new ExpectedError() { ErrorCode = 20, Line = 91 },
                    new ExpectedError() { ErrorCode = 20, Line = 98 },
                    new ExpectedError() { ErrorCode = 20, Line = 105 },
                    new ExpectedError() { ErrorCode = 20, Line = 112 },
                    new ExpectedError() { ErrorCode = 20, Line = 119 },
                    new ExpectedError() { ErrorCode = 20, Line = 126 },
                    new ExpectedError() { ErrorCode = 20, Line = 133 },
                    new ExpectedError() { ErrorCode = 20, Line = 140 },
                    new ExpectedError() { ErrorCode = 21, Line = 146 },
                    new ExpectedError() { ErrorCode = 21, Line = 152 },
                    new ExpectedError() { ErrorCode = 21, Line = 158 },
                    new ExpectedError() { ErrorCode = 21, Line = 164 },
                    new ExpectedError() { ErrorCode = 21, Line = 170 },
                    new ExpectedError() { ErrorCode = 21, Line = 176 },
                    new ExpectedError() { ErrorCode = 21, Line = 182 },
                    new ExpectedError() { ErrorCode = 21, Line = 188 },
                    new ExpectedError() { ErrorCode = 21, Line = 194 },
                    new ExpectedError() { ErrorCode = 21, Line = 200 },
                    new ExpectedError() { ErrorCode = 21, Line = 206 },
                    new ExpectedError() { ErrorCode = 7, Line = 213 },
                    new ExpectedError() { ErrorCode = 7, Line = 220 },
                    new ExpectedError() { ErrorCode = 7, Line = 227 },
                    new ExpectedError() { ErrorCode = 7, Line = 234 },
                    new ExpectedError() { ErrorCode = 7, Line = 241 },
                    new ExpectedError() { ErrorCode = 7, Line = 248 },
                    new ExpectedError() { ErrorCode = 7, Line = 255 },
                    new ExpectedError() { ErrorCode = 7, Line = 262 },
                    new ExpectedError() { ErrorCode = 7, Line = 269 },
                    new ExpectedError() { ErrorCode = 7, Line = 276 },
                    new ExpectedError() { ErrorCode = 7, Line = 283 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/TypeError/Conditionals.hopl",
                ErrorType = "Type Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 24, Line = 5 },
                    new ExpectedError() { ErrorCode = 24, Line = 12 },
                    new ExpectedError() { ErrorCode = 24, Line = 18 },
                    new ExpectedError() { ErrorCode = 24, Line = 25 },
                    new ExpectedError() { ErrorCode = 24, Line = 31 },
                    new ExpectedError() { ErrorCode = 24, Line = 38 },
                    new ExpectedError() { ErrorCode = 24, Line = 44 },
                    new ExpectedError() { ErrorCode = 24, Line = 51 },
                    new ExpectedError() { ErrorCode = 25, Line = 57 },
                    new ExpectedError() { ErrorCode = 25, Line = 63 },
                    new ExpectedError() { ErrorCode = 25, Line = 69 },
                    new ExpectedError() { ErrorCode = 25, Line = 75 },
                    new ExpectedError() { ErrorCode = 25, Line = 81 },
                    new ExpectedError() { ErrorCode = 25, Line = 87 },
                    new ExpectedError() { ErrorCode = 25, Line = 93 },
                    new ExpectedError() { ErrorCode = 25, Line = 99 },
                    new ExpectedError() { ErrorCode = 25, Line = 105 },
                    new ExpectedError() { ErrorCode = 25, Line = 111 },
                    new ExpectedError() { ErrorCode = 25, Line = 117 },
                    new ExpectedError() { ErrorCode = 6, Line = 123 },
                    new ExpectedError() { ErrorCode = 6, Line = 130 },
                    new ExpectedError() { ErrorCode = 6, Line = 136 },
                    new ExpectedError() { ErrorCode = 6, Line = 142 },
                    new ExpectedError() { ErrorCode = 6, Line = 148 },
                    new ExpectedError() { ErrorCode = 6, Line = 155 },
                    new ExpectedError() { ErrorCode = 6, Line = 161 },
                    new ExpectedError() { ErrorCode = 6, Line = 167 },
                    new ExpectedError() { ErrorCode = 6, Line = 173 },
                    new ExpectedError() { ErrorCode = 6, Line = 180 },
                    new ExpectedError() { ErrorCode = 6, Line = 186 },
                    new ExpectedError() { ErrorCode = 6, Line = 192 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/ExplorationError/GlobalVars.hopl",
                ErrorType = "Explore Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 1, Line = 4 },
                    new ExpectedError() { ErrorCode = 1, Line = 7 },
                    new ExpectedError() { ErrorCode = 1, Line = 10 },
                    new ExpectedError() { ErrorCode = 1, Line = 13 },
                    new ExpectedError() { ErrorCode = 1, Line = 16 },
                    new ExpectedError() { ErrorCode = 1, Line = 19 },
                    new ExpectedError() { ErrorCode = 1, Line = 22 },
                    new ExpectedError() { ErrorCode = 1, Line = 25 },
                    new ExpectedError() { ErrorCode = 1, Line = 28 },
                    new ExpectedError() { ErrorCode = 1, Line = 31 },
                    new ExpectedError() { ErrorCode = 1, Line = 34 },
                    new ExpectedError() { ErrorCode = 1, Line = 37 },
                    new ExpectedError() { ErrorCode = 1, Line = 40 },
                    new ExpectedError() { ErrorCode = 1, Line = 43 },
                    new ExpectedError() { ErrorCode = 1, Line = 46 },
                    new ExpectedError() { ErrorCode = 1, Line = 49 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/ExplorationError/Functions.hopl",
                ErrorType = "Explore Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 2, Line = 4 },
                    new ExpectedError() { ErrorCode = 2, Line = 7 },
                    new ExpectedError() { ErrorCode = 2, Line = 10 },
                    new ExpectedError() { ErrorCode = 2, Line = 13 },
                    new ExpectedError() { ErrorCode = 2, Line = 16 },
                    new ExpectedError() { ErrorCode = 2, Line = 19 },
                    new ExpectedError() { ErrorCode = 2, Line = 22 },
                    new ExpectedError() { ErrorCode = 2, Line = 25 },
                    new ExpectedError() { ErrorCode = 2, Line = 28 },
                    new ExpectedError() { ErrorCode = 2, Line = 31 },
                    new ExpectedError() { ErrorCode = 2, Line = 34 },
                    new ExpectedError() { ErrorCode = 2, Line = 37 },
                    new ExpectedError() { ErrorCode = 2, Line = 40 },
                    new ExpectedError() { ErrorCode = 2, Line = 43 },
                    new ExpectedError() { ErrorCode = 2, Line = 46 },
                    new ExpectedError() { ErrorCode = 2, Line = 49 },
                    new ExpectedError() { ErrorCode = 3, Line = 51 },
                    new ExpectedError() { ErrorCode = 3, Line = 53 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/ExplorationError/Alias.hopl",
                ImportPaths = new string[1] {"./InternalTests/ExplorationError/Libraries"},
                ErrorType = "Explore Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 4, Line = 2 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/PreparationError/UnknownImport.hopl",
                ErrorType = "Prepare Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 1, Line = 0 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/PreparationError/ImportRequired.hopl",
                ImportPaths = new string[1] {"./InternalTests/PreparationError/Libraries"},
                ErrorType = "Prepare Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 2, Line = 0 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/PreparationError/RecursiveGlobalEntity.hopl",
                ErrorType = "Prepare Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 3, Line = 0 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/PreparationError/AwaitDependencyDirect.hopl",
                ErrorType = "Prepare Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 4, Line = 0 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/PreparationError/AwaitDependencyIndirect.hopl",
                ErrorType = "Prepare Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 4, Line = 0 },
                }
            },
        };

        public static ErroneousFile[] RuntimeErrorFiles { get; set; } =
        {
            new ErroneousFile()
            {
                File = "./InternalTests/RuntimeError/FunctionVarNotSet.hopl",
                ErrorType = "Runtime Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 1, Line = 8 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/RuntimeError/IndexOutOfRangeHigh.hopl",
                ErrorType = "Runtime Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 2, Line = 8 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/RuntimeError/IndexOutOfRangeLow.hopl",
                ErrorType = "Runtime Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 2, Line = 8 },
                }
            },
            new ErroneousFile()
            {
                File = "./InternalTests/RuntimeError/DivisionByZero.hopl",
                ErrorType = "Runtime Errors",
                ExpectedErrors = new ExpectedError[]
                {
                    new ExpectedError() { ErrorCode = 3, Line = 7 },
                }
            }
        };
    }
}
