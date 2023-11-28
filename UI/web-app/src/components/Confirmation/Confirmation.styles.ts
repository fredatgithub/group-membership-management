// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

import {
    type IConfirmationStyleProps,
    type IConfirmationStyles,
} from './Confirmation.types';

export const getStyles = (props: IConfirmationStyleProps): IConfirmationStyles => {
    const { className, theme } = props;

    return {
        root: [{
        }, className],
        ConfirmationContainer: {
            display: 'flex',
            flexDirection: 'column',
            gap: 36,
        },
        cardHeader: {
            display: 'flex',
            flexDirection: 'row',
            alignItems: 'space-between',
        },
        cardTitle: {
            textTransform: "uppercase",
            flex: 1,
            fontSize: 16,
            fontStyle: 'normal',
            fontWeight: 600,
            lineHeight: '22px',
        },
        itemTitle: {
            fontSize: 14,
            fontWeight: 600
        },
        itemData: {
            paddingTop: 10,
            fontSize: 14
        },
        endpointsContainer: {
            display: 'flex',
            flexDirection: 'column',
            gap: 23
        },
    };
};
