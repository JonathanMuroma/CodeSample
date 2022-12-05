<?php
$section = get_query_var('section');
if ($section == null) {
    $section = $template_args;
}

?>
<style>
    <?php
    if ($section['button_active_color']) {
    ?>.active-button {
        background: <?= $section['button_active_color'] ?> !important;
    }

    .map-form-button:hover {
        background: <?= $section['button_active_color'] ?> !important;
    }

    <?php
    } else {
    ?>.active-button {
        background: #ececec !important;
    }

    .map-form-button:hover {
        background: #ececec !important;
    }

    <?php
    }
    ?><?php
        if ($section['button_active_text_color']) {
        ?>.active-button {
        color: <?= $section['button_active_text_color'] ?> !important;
    }

    .map-form-button:hover {
        color: <?= $section['button_active_text_color'] ?> !important;
    }

    <?php
        }
    ?><?php
        if ($section['search_button_color']) {
        ?>.map-form-search-button {
        background: <?= $section['button_active_color'] ?> !important;
    }

    <?php
        }
        if ($section['search_button_text_color']) {
    ?>.map-form-search-button>p {
        color: <?= $section['search_button_text_color'] ?> !important;
    }

    <?php
        }
    ?>
</style>
<script>
    function MapButtonClicked(btn) {
        if (window.filters.includes(btn)) {
            // Remove filter
            window.filters = window.filters.filter(e => e !== btn);
        } else {
            window.filters.push(btn);
        }
        window.refreshFormMapPins();
    }

    function ChangeMapCentre() {
        const address = encodeURI($('#fplace').val())
        $.ajax({
            url: 'https://maps.googleapis.com/maps/api/geocode/json?address=' + address + '&key=<?= carbon_get_theme_option('google_maps_api_key') ?>',
            success: function(res) {
                const coords = res.results[0].geometry.location;
                window.formMap.setCenter(new google.maps.LatLng(coords.lat, coords.lng));
                window.formMap.setZoom(10);
            },
            error: function(res) {
                console.error(res);
            }
        })
    }
    document.onkeypress = function(e) {
        e = e || window.event;
        // use e.keyCode
        if (e.key === 'Enter') {
            ChangeMapCentre();
        }
    };
</script>
<div class="container map-form">
    <div class="d-flex flex-row map-form-content-holder">
        <div class="map-form-content-left">
            <div class="map-form-button-section">
                <p>Valitse tyyppi</p>
                <div class="d-flex flex-column flex-wrap map-form-button-container <?php if(carbon_get_theme_option('use_peugeot_styling') == "2") { echo "pegeButCont"; } ?>">
                    <button onClick="MapButtonClicked('wjalleenmyyja');" class="map-form-button jalleenmyynti">J&auml;lleenmyyj&auml;t</button>
                    <button onClick="MapButtonClicked('whuoltoliike');" class="map-form-button huoltoliikkeet">Huoltoliikkeet</button>
                </div>
            </div>
            <?php
            if ($section['have_brand_options'] == true) {
            ?>
                <div class="map-form-button-section">
                    <p>Valitse tyyppi</p>
                    <div class="d-flex flex-column flex-wrap map-form-button-container">
                        <button onClick="MapButtonClicked('peugeot');" class="map-form-button peugeot">Peugeot</button>
                        <button onClick="MapButtonClicked('drac');" class="map-form-button drac">Drac</button>
                        <button onClick="MapButtonClicked('aixam');" class="map-form-button aixam">Aixam</button>
                        <button onClick="MapButtonClicked('conan');" class="map-form-button conan">Conan</button>
                        <button onClick="MapButtonClicked('genergia');" class="map-form-button genergia">Genergia</button>
                    </div>
                </div>
            <?php
            }
            ?>
            <div class="map-form-button-section">
                <p>Sy&ouml;t&auml; paikkakunta</p>
                <div class="d-flex flex-row map-form-input-field-holder">
                    <input type="text" id="fplace" name="fplace" placeholder="Esim. Helsinki">
                    <button onClick="ChangeMapCentre()" class="map-form-search-button <?php if(carbon_get_theme_option('use_peugeot_styling') == "2") { echo "pegeBut"; } ?>">
                        <p>Hae</p>
                    </button>
                </div>
            </div>
        </div>
        <div class="map-form-content-right">
            <div id="form-map" style="height: 100%; width: 100%;"></div>
        </div>
    </div>
</div>


<!-- Google Maps -->

<script>
    function initMap() {
        //form map
        const locations = [
            <?php
            switch_to_blog((int)carbon_get_theme_option('site_num')); //switch site

            $loop = new WP_Query(array('post_type' => 'map_location', 'posts_per_page' => -1));
            if ($loop->have_posts()) :
                while ($loop->have_posts()) : $loop->the_post();
                    $onBrand = false;
                    if ($section['brand'] == 'all') {
                        $onBrand = true;
                    } else {
                        if ($section['brand'] == 'peugeot') {
                            if (carbon_get_the_post_meta('peugeot_huolto') == 'TRUE' || carbon_get_the_post_meta('peugeot_myynti') == 'TRUE') {
                                $onBrand = true;
                            }
                        } else if ($section['brand'] == 'drac') {
                            if (carbon_get_the_post_meta('drac_huolto') == 'TRUE' || carbon_get_the_post_meta('drac_myynti') == 'TRUE') {
                                $onBrand = true;
                            }
                        } else if ($section['brand'] == 'aixam') {
                            if (carbon_get_the_post_meta('aixam_huolto') == 'TRUE' || carbon_get_the_post_meta('aixam_myynti') == 'TRUE') {
                                $onBrand = true;
                            }
                        } else if ($section['brand'] == 'conan') {
                            if (carbon_get_the_post_meta('conan_huolto') == 'TRUE' || carbon_get_the_post_meta('conan_myynti') == 'TRUE') {
                                $onBrand = true;
                            }
                        } else if ($section['brand'] == 'genergia') {
                            if (carbon_get_the_post_meta('genergia_huolto') == 'TRUE' || carbon_get_the_post_meta('genergia_myynti') == 'TRUE') {
                                $onBrand = true;
                            }
                        }
                    }
                    if ($onBrand) {
            ?> {
                            "location": {
                                "lat": <?= carbon_get_the_post_meta('latitude') ?>,
                                "lng": <?= carbon_get_the_post_meta('longitude') ?>
                            },
                            "tags": {
                                "peugeot_h": "<?= carbon_get_the_post_meta('peugeot_huolto') ?>",
                                "peugeot_m": "<?= carbon_get_the_post_meta('peugeot_myynti') ?>",
                                "drac_h": "<?= carbon_get_the_post_meta('drac_huolto') ?>",
                                "drac_m": "<?= carbon_get_the_post_meta('drac_myynti') ?>",
                                "aixam_h": "<?= carbon_get_the_post_meta('aixam_huolto') ?>",
                                "aixam_m": "<?= carbon_get_the_post_meta('aixam_myynti') ?>",
                                "conan_h": "<?= carbon_get_the_post_meta('conan_huolto') ?>",
                                "conan_m": "<?= carbon_get_the_post_meta('conan_myynti') ?>",
                                "genergia_h": "<?= carbon_get_the_post_meta('genergia_huolto') ?>",
                                "genergia_m": "<?= carbon_get_the_post_meta('genergia_myynti') ?>"
                            },
                            "info": "<?= get_the_title() ?>, <br/> <?= carbon_get_the_post_meta('address') ?>, <?= carbon_get_the_post_meta('postalcode') ?>, <br/> <?= carbon_get_the_post_meta('postaloffice') ?>, <br/> <?= carbon_get_the_post_meta('phone') ?> <?php if(carbon_get_the_post_meta('website') !== null && carbon_get_the_post_meta('website') != '') { echo ' <br/> <br/>'; }?> <a href='<?= carbon_get_the_post_meta('website') ?>' target='_blank'><?= carbon_get_the_post_meta('website') ?></a>"
                        },
            <?php

                    }
                endwhile;
            endif;
            restore_current_blog();
            ?>
        ];

        window.formMap = new google.maps.Map(document.getElementById("form-map"), {
            zoom: 4.7,
            center: {
                lat: 65.25,
                lng: 25.748152
            },
            mapTypeId: 'roadmap'
        });
        window.pins = [];
        window.filters = [];
        window.infoWindows = [];

        window.refreshFormMapPins = function() {
            for (var i = 0; i < window.pins.length; i++) {
                window.pins[i].setMap(null);
                window.infoWindows[i].setMap(null);
            }

            window.pins.length = 0;
            locations.map(function(location) {
                const filters = window.filters;
                var shouldShow = true;
                if (filters.length > 0) {
                    filters.sort();
                   
                    var brandFilters = [];
                    filters.forEach(filter => {
                        var found = false;
                        if(filter == 'wjalleenmyyja') {

                            if(brandFilters.length > 0) {
                                for (var i = 0; i < brandFilters.length; i++) {
                                    if(brandFilters[i] == 'peugeot' && location.tags.peugeot_m == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'drac' && location.tags.drac_m == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'aixam' && location.tags.aixam_m == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'conan' && location.tags.conan_m == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'genergia' && location.tags.genergia_m == 'TRUE') {
                                        found = true;
                                    } else {
                                        found = false;
                                        break;
                                    }
                                }
                            } else if(location.tags.peugeot_m == 'TRUE' || location.tags.drac_m == 'TRUE' || location.tags.aixam_m == 'TRUE' || location.tags.conan_m == 'TRUE' || location.tags.genergia_m == 'TRUE') {
                                found = true;
                            }
                        } else if(filter == 'whuoltoliike') {
                            if(brandFilters.length > 0) {
                                for (var i = 0; i < brandFilters.length; i++) {
                                    if(brandFilters[i] == 'peugeot' && location.tags.peugeot_h == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'drac' && location.tags.drac_h == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'aixam' && location.tags.aixam_h == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'conan' && location.tags.conan_h == 'TRUE') {
                                        found = true;
                                    } else if (brandFilters[i] == 'genergia' && location.tags.genergia_h == 'TRUE') {
                                        found = true;
                                    } else {
                                        found = false;
                                        break;
                                    }
                                }                             
                            } else if(location.tags.peugeot_h == 'TRUE' || location.tags.drac_h == 'TRUE' || location.tags.aixam_h == 'TRUE' || location.tags.conan_h == 'TRUE' || location.tags.genergia_h == 'TRUE') {
                                found = true;
                            }
                        } else if(filter == 'peugeot') {
                            if(location.tags.peugeot_h == 'TRUE' || location.tags.peugeot_m == 'TRUE' ) {
                                found = true;
                                brandFilters.push('peugeot');
                            }
                        } else if(filter == 'drac') {
                            if(location.tags.drac_h == 'TRUE' || location.tags.drac_m == 'TRUE' ) {
                                found = true;
                                brandFilters.push('drac');
                            }
                        } else if(filter == 'aixam') {
                            if(location.tags.aixam_h == 'TRUE' || location.tags.aixam_m == 'TRUE' ) {
                                found = true;
                                brandFilters.push('aixam');
                            }
                        } else if(filter == 'conan') {
                            if(location.tags.conan_h == 'TRUE' || location.tags.conan_m == 'TRUE' ) {
                                found = true;
                                brandFilters.push('conan');
                            }
                        } else if(filter == 'genergia') {
                            if(location.tags.genergia_h == 'TRUE' || location.tags.genergia_m == 'TRUE' ) {
                                found = true;
                                brandFilters.push('genergia');
                            }
                        } 
                        if (!found) {
                            shouldShow = false;
                        }
                    })
                }
                if (shouldShow) {
                    var info = new google.maps.InfoWindow({
                        content: location.info,
                        position: location.location,
                        maxWidth: 500
                    });

                    var pin = new google.maps.Marker({
                        position: location.location,
                        visible: true,
                        map: window.formMap
                    });
                    pin.addListener('click', function() {

                        window.infoWindows.forEach(function(infoWindow) {
                            infoWindow.close();
                        });

                        info.open(window.formMap, pin);
                    });
                    window.infoWindows.push(info);
                    window.pins.push(pin);
                }
            })
        }
        window.refreshFormMapPins();
    }
</script>

<!-- Google maps -->
<script async defer src="https://maps.googleapis.com/maps/api/js?key=<?= carbon_get_theme_option('google_maps_api_key') ?>&callback=initMap"></script>