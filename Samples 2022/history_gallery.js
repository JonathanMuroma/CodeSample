import React, { useState, useEffect } from "react";
import { Box, Image } from "@chakra-ui/react";

import HistoryGalleryImage from "../partials/history_gallery_image";
import HistoryGalleryText from "../partials/history_gallery_text";

import arrowUpUrl from "../../../static/images/arrow_up.svg";
import arrowDownUrl from "../../../static/images/arrow_downwards.svg";

function HistoryGallery({ block }) {
  const [blackCircleIndex, setBlackCircleIndex] = useState(0);

  let listener = null;
  const [scrollState, setScrollState] = useState(0);
  let prevPos = -1;
  useEffect(() => {
    if (typeof document !== "undefined") {
      listener = document.addEventListener("scroll", (e) => {
        var scrolled = document.scrollingElement.scrollTop;

        const height = document.getElementById("historyBlock" + 0).scrollHeight;

        const scrollPos = Math.round(scrolled / height) - 1;

        if (prevPos !== scrollPos) {
          prevPos = scrollPos;
          if (prevPos < 0) {
            return;
          } else if (prevPos > block.columns.length - 1) {
            return;
          } else {
            setBlackCircleIndex(prevPos);
          }
        }
      });
      return () => {
        document.removeEventListener("scroll", listener);
      };
    }
  }, [scrollState]);

  function moveBlackCircle(num) {
    var element = document.getElementById("historyBlock" + num);
    if (num < 0) {
      return;
    } else if (num > block.columns.length - 1) {
      return;
    }
    setBlackCircleIndex(num);
    element.scrollIntoView({ behavior: "smooth" });
  }

  return (
    <Box
      width="100%"
      borderTop="1px"
      borderBottom="1px"
      paddingLeft={{ base: "10%", sm: "0" }}
      borderColor="#000000"
    >
      <Box
        width="100%"
        paddingLeft={{ base: "17%", sm: "0" }}
        paddingTop={{ base: "70px", sm: "0" }}
        borderLeft={{ base: "1px", sm: "0" }}
        borderColor="#000000"
        position="relative"
      >
        {block.columns.map((column, idx) => (
          <Box key={idx} id={"historyBlock" + idx}>
            <Box
              position="absolute"
              left="-18px"
              zIndex="10"
              display={{ base: "block", sm: "none" }}
            >
              {/*Mobile CIRCLE & BUTTONS*/}
              <Box
                background={blackCircleIndex == idx ? "black" : "white"}
                border="1px"
                borderColor="#000000"
                borderRadius="50%"
                height="36px"
                width="36px"
              ></Box>
              <Box
                as="button"
                onClick={() => moveBlackCircle(parseInt(idx, 10) - 1)}
                background="white"
                border="1px"
                borderColor="#000000"
                borderRadius="20px"
                height="53px"
                width="36px"
                display={blackCircleIndex == idx ? "flex" : "none"}
                justifyContent="center"
                alignItems="center"
                marginTop="10px"
              >
                <Image src={arrowUpUrl} />
              </Box>
              <Box
                as="button"
                onClick={() => moveBlackCircle(parseInt(idx, 10) + 1)}
                background="white"
                border="1px"
                borderColor="#000000"
                borderRadius="20px"
                height="53px"
                width="36px"
                display={blackCircleIndex == idx ? "flex" : "none"}
                justifyContent="center"
                alignItems="center"
                marginTop="10px"
              >
                <Image src={arrowDownUrl} />
              </Box>
            </Box>
            <Box
              display="flex"
              flexDirection={{
                base: idx % 2 == 1 ? "column-reverse" : "column",
                sm: "row",
              }}
              alignItems={{ base: "end", sm: "unset" }}
              height={{
                base: "auto",
                sm: "50vw",
              }}
              position="relative"
              paddingBottom={{ base: "70px", sm: "0" }}
              id={"historyColumn" + idx}
            >
              {idx % 2 == 0 ? (
                <HistoryGalleryImage
                  image={column.image}
                  title={column.image_title}
                />
              ) : (
                <HistoryGalleryText text={column.text} />
              )}
              <Box
                display={{ base: "none", sm: "flex" }}
                height="100%"
                justifyContent="center"
                alignItems="center"
                borderLeft="1px"
                borderColor="#000000"
              >
                <Box position="absolute" zIndex="10">
                  {/*TOP BUTTON */}
                  <Box
                    display={blackCircleIndex == idx ? "flex" : "none"}
                    as="button"
                    marginBottom="10px"
                    onClick={() => moveBlackCircle(parseInt(idx, 10) - 1)}
                    background="white"
                    border="1px"
                    borderColor="#000000"
                    borderRadius="20px"
                    height="53px"
                    width="36px"
                    justifyContent="center"
                    alignItems="center"
                  >
                    <Image src={arrowUpUrl} />
                  </Box>
                  {/*WHITE CIRCLE */}
                  <Box
                    background={blackCircleIndex == idx ? "black" : "white"}
                    border="1px"
                    borderColor="#000000"
                    borderRadius="50%"
                    height="36px"
                    width="36px"
                  ></Box>
                  {/*BOTTOM BUTTON */}
                  <Box
                    display={blackCircleIndex == idx ? "flex" : "none"}
                    as="button"
                    marginTop="10px"
                    onClick={() => moveBlackCircle(parseInt(idx, 10) + 1)}
                    background="white"
                    border="1px"
                    borderColor="#000000"
                    borderRadius="20px"
                    height="53px"
                    width="36px"
                    justifyContent="center"
                    alignItems="center"
                  >
                    <Image src={arrowDownUrl} />
                  </Box>
                </Box>
              </Box>
              {idx % 2 == 0 ? (
                <HistoryGalleryText text={column.text} />
              ) : (
                <HistoryGalleryImage
                  image={column.image}
                  title={column.image_title}
                />
              )}
            </Box>
          </Box>
        ))}
      </Box>
    </Box>
  );
}

export default HistoryGallery;
