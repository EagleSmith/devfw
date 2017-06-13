//
//���� �� Table��
//�汾:1.0
//ʱ�䣺2012-09-22
//

jr.extend({
    table: {
        //================ ��̬����� ====================//
        dynamic: function (table, multSelected, clickFunc) {
            if (table && table.nodeName === "TABLE") {

                //����table����ʽ
                table.className += ' ui-table';

                //����th�ķָ�
                var ths = table.getElementsByTagName('TH');
                window.jr.each(ths, function (i, e) {
                    if (i != ths.length - 1) {
                        if ((e.getElementsByClassName ? e.getElementsByClassName('th-split') : document.getElementsByClassName('th-split', e)).length == 0) {
                            var split = document.createElement("SPAN");
                            split.className = 'th-split';
                            e.appendChild(split);
                        }
                    }
                });


                var rows = table.getElementsByTagName("tr");
                for (var i = 0; i < rows.length; i++) {
                    if (i % 2 == 1) if (!rows[i].className) rows[i].className = 'even';
                    rows[i].onmouseover = function () {
                        if (this.className.indexOf('selected') == -1) {
                            this.className = this.className.indexOf('even') != -1 ? "hover even" : "hover";
                        }
                    };
                    rows[i].onmouseout = function () {
                        if (this.className.indexOf('selected') == -1) {
                            this.className = this.className.indexOf("even") == -1 ? "" : "even";
                        }
                    };

                    rows[i].onclick = (function (_rows, _this, _multSel) {
                        return function () {
                            var trs = new Array();

                            //ȡ������ѡ��
                            window.jr.each(_rows, function (i, e) {
                                if (!_multSel) {
                                    if (e != _this) {
                                        e.className = e.className.indexOf("even") == -1 ? "" : "even";
                                    }
                                }

                                //��ѡ�еļ��뵽����
                                if (e.className.indexOf('selected') != -1) {
                                    trs.push(e);
                                }
                            });

                            //δѡ������£�����ѡ��
                            if (_this.className.indexOf('selected') == -1) {

                                _this.className = _this.className.indexOf("even") == -1 ? "selected" : "selected even";

                                //����ǰ�м��뵽����
                                trs.push(_this);
                            }

                            //��������¼�
                            if (clickFunc) { clickFunc(trs); }
                        };
                    })(rows, rows[i], multSelected);
                }
            }
        }
    }
});