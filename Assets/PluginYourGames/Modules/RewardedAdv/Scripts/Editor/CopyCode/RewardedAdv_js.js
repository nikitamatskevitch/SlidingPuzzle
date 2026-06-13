function RewardedAdvShow(id) {
    try {
        if (ysdk == null) {
            LogStyledMessage('Cancel RewardedAdvShow: SDK is not initialized');
            return;
        }
        if (nowFullAdOpen == true) {
            LogStyledMessage('Cancel RewardedAdvShow: The advertisement is already open');
            return;
        }

        ysdk.adv.showRewardedVideo({
            callbacks: {
                onOpen: () => {
                    LogStyledMessage('Open Rewarded Adv');
                    nowFullAdOpen = true;
                    if (initGame === true) {
                        YG2Instance('OpenRewardedAdv');
                    }
                },
                onRewarded: () => {
                    LogStyledMessage('Rewarded Adv reward');
                    if (initGame === true) {
                        YG2Instance('RewardAdv', id);
                    }
                },
                onClose: () => {
                    LogStyledMessage('Close Rewarded Adv');
                    nowFullAdOpen = false;
                    if (initGame === true) {
                        YG2Instance('CloseRewardedAdv');
                    }
                    FocusGame();
                },
                onError: (error) => {
                    console.error('Error Rewarded Adv', error);
                    nowFullAdOpen = false;
                    YG2Instance('ErrorRewardedAdv');
                    FocusGame();
                }
            }
        });
    }
    catch (e) {
        console.error('CRASH Rewarded Adv Show: ', e.message);
    }
}
