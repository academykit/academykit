import {
  Breadcrumbs,
  Anchor,
  Divider,
  MantineStyleSystemProps,
} from "@mantine/core";
import { Link, useLocation } from "react-router-dom";

type ItemsProps = {
  items: {
    title: string;
    href: string;
  }[];
  hide: number | null;
};

const getItems = ({ items, hide }: ItemsProps) => {
  const finalItem = items.map((item, index) => (
    <Anchor size={"sm"} to={item.href} component={Link} key={index}>
      {item.title}
    </Anchor>
  ));
  if (hide) {
    finalItem.splice(hide, 1);
  }
  return finalItem;
};
/**
 * Makes breadcrumb from url path
 * @param hide to hide element of that particular index
 * @returns ReactElement
 */
const Breadcrumb = ({
  hide = null,
  start,
  py = 15,
}: {
  hide?: number | null;
  py?: MantineStyleSystemProps["py"];
  start?: { title: string; href: string };
}) => {
  const routes = useLocation();
  const pathNames = routes.pathname.split("/").splice(1);

  const newPath = pathNames.map((x, i) => {
    var elementTo = i + 1;
    if (x === "courses") x = "trainings";
    const href = "/" + [...pathNames].splice(0, elementTo).join("/");
    const a = {
      title: x,
      href,
    };
    return a;
  });

  return (
    <>
      <Breadcrumbs py={py} pb={5}>
        {getItems({ items: start ? [start, ...newPath] : newPath, hide })}
      </Breadcrumbs>
      <Divider />
    </>
  );
};

export default Breadcrumb;
