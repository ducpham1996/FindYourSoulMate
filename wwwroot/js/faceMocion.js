$.fn.extend({
    faceMocion: function (opciones, _id) {
        var faceMocion = this;
        var NombreSelector = "Selector";
        var DescripcionFace = "--";
        defaults = {
            emociones: [
                { "emocion": "like", "TextoEmocion": "I like it" },
                { "emocion": "love", "TextoEmocion": "I love" },
                { "emocion": "angry", "TextoEmocion": "It bothers me" },
                { "emocion": "scare", "TextoEmocion": "it scares" },
                { "emocion": "haha", "TextoEmocion": "I enjoy" },
                { "emocion": "sad", "TextoEmocion": "It saddens" },
                { "emocion": "amaze", "TextoEmocion": "It amazes me" },
                { "emocion": "suprise", "TextoEmocion": "I am glad" }
            ]
        };
        var opciones = $.extend({}, defaults, opciones);

        $(faceMocion).each(function (index) {
            $(this).attr("class", $(faceMocion).attr("class") + " " + _id);
            var EstadoInicial = "Like";
            if ($(this).val() != "") {
                EstadoInicial = $(this).val();
            } else {
                $(this).val('Like');
            }
            DescripcionFace = EstadoInicial;
            ElementoIniciar = '';
            ElementoIniciar = ElementoIniciar + '<div dato-descripcion="' + DescripcionFace + '" ';
            ElementoIniciar = ElementoIniciar + 'id-referencia="' + _id;
            ElementoIniciar = ElementoIniciar + '"  class="' + NombreSelector;
            ElementoIniciar = ElementoIniciar + ' selectorFace ' + EstadoInicial + '">Like</div>'
            $(this).before(ElementoIniciar);
        });


        $(document).ready(function () {
            BarraEmociones = '<div class="faceMocion">';
            $.each(opciones.emociones, function (index, emo) {
                BarraEmociones = BarraEmociones + '<div dato-descripcion="' + emo.TextoEmocion;
                BarraEmociones = BarraEmociones + '" class="' + emo.emocion + '"></div>';
            });
            BarraEmociones = BarraEmociones + '</div>';
            //BarraEmociones = BarraEmociones + ''
            $(document.body).append(BarraEmociones);
            $('.faceMocion div').hover(function () {
                var title = $(this).attr('dato-descripcion');
                $(this).data('tipText', title).removeAttr('dato-descripcion');
                $('<p class="MensajeTexto"></p>').text(title).appendTo('body').fadeIn('slow');
                var emo = $(this).attr('class');
                //$(this).attr("onClick", "onLike('" + _id + "','" + emo + "')");
                $(this).prop("onclick", null).off('click');
            }, function () {
                $(this).attr('dato-descripcion', $(this).data('tipText'));
                $('.MensajeTexto').remove();
            }).mousemove(function (e) {
                var RatonX = e.pageX - 20; var RatonY = e.pageY - 60;
                $('.MensajeTexto').css({ top: RatonY, left: RatonX })
                var id = $('.faceMocion').attr('id');
                var action = $('.faceMocion').attr('property');
                if (action == "c") {
                    $(this).attr('onclick', "onLike('" + id + "','" + $(this).attr('class') + "')");
                }
                if (action == "p") {
                    $(this).attr('onclick', "onPostLike('" + id + "','" + $(this).attr('class') + "')");
                }
            });
        });
        var timer;
        var delay = 1000;
        $('.' + NombreSelector).hover(function (e) {
            //SelectorEmocion = $(this);
            //timer = setTimeout(function () {
            //    // do your stuff here
            //    var RatonX = e.pageX - 20; var RatonY = e.pageY - 60;
            //    $(".faceMocion").css({ top: RatonY, left: RatonX });
            //    $(".faceMocion").show();
            //}, delay);  
            //$(".faceMocion div").click(function () {
            //    SelectorEmocion.attr("class", NombreSelector + " selectorFace  " + $(this).attr('class'));
            //    SelectorEmocion.empty();
            //    ElInputSeleccionado = SelectorEmocion.attr("id-referencia");
            //    $("." + ElInputSeleccionado).val($(this).attr('class'));

            //});

        }, function () {
            // on mouse out, cancel the timer
            clearTimeout(timer);
        });
        $(document).mouseup(function (e) {
            //alert($(event.target).attr('class'));
            $(".faceMocion").hide();
        });
        $(faceMocion).hide();
    }
});