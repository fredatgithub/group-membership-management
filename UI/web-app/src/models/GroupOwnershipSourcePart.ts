// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import { SourcePartType } from "./ISourcePart";

export type GroupOwnershipSourcePart = {
    type: SourcePartType.GroupOwnership;
    source: string[];
    exclusionary?: boolean;
};