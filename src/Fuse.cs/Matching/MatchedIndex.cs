﻿/*
* Copyright 2008 Google Inc. All Rights Reserved.
* Author: fraser@google.com (Neil Fraser)
* Author: anteru@developer.shelter13.net (Matthaeus G. Chajdas)
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
* Diff Match and Patch
* http://code.google.com/p/google-diff-match-patch/
*/

namespace FuseCs.Matching
{
    public class MatchedIndex
    {
        internal MatchedIndex(int start, int end)
        {
            Start = start;
            End = end;
        }
        public int Start { get; set; }
        public int End { get; set; }
    }

    /**
     * Class containing the diff, match and patch methods.
     * Also Contains the behaviour settings.
     */
}
