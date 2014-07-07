export module Implementation.Binding {
    var iso6801: RegExp = /(\d{4})-(\d\d)-(\d\d)T(\d\d):(\d\d):(\d\d)(\.\d+)?(Z|([+\-])(\d\d):(\d\d))/;
    var numericMatchIndexes: number[] = [1, 2, 3, 4, 5, 6, 10, 11];

    function checkForDates(json: Object): void {
        if (!json) {
            return;
        }

        jQuery.each(json, (index, value) => {
            var matches,
                mSeconds,
                offset;

            if (typeof value !== 'string') {
                checkForDates(value);
                return;
            }

            matches = value.match(iso6801);
            if (!matches) {
                return;
            }

            jQuery.each(numericMatchIndexes, (key, match) => matches[match] = parseInt(matches[match], 10));
            matches[7] = parseFloat(matches[7]);

            mSeconds = Date.UTC(matches[1], matches[2] - 1, matches[3], matches[4], matches[5], matches[6]);
            if (matches[7] > 0) {
                mSeconds += Math.round(matches[7] * 1000);
            }

            if (matches[8] !== 'Z' && matches[10]) {
                offset = matches[10] * 60 * 60 * 1000;
                if (matches[11]) {
                    offset += matches[11] * 60 * 1000;
                }
                if (matches[9] === "-") {
                    mSeconds -= offset;
                } else {
                    mSeconds += offset;
                }
            }

            json[index] = new Date(mSeconds);
        });
    }

    jQuery.ajaxSettings.converters['text json'] = (data) => {
        var json: Object;

        if (!data) {
            return data;
        }

        json = jQuery.parseJSON(data);
        checkForDates(json);
        return json;
    };
}