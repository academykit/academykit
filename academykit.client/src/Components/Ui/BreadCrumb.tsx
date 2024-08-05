import useNav from '@hooks/useNav';
import { Anchor, Breadcrumbs, Divider, MantineStyleProps } from '@mantine/core';
import { useTranslation } from 'react-i18next';
import { Link, useLocation } from 'react-router-dom';

type ItemsProps = {
  items: {
    title: string;
    href: string;
  }[];
  hide: number | null;
};

const getItems = ({ items, hide }: ItemsProps) => {
  const { t } = useTranslation();
  const finalItem = items.map((item, index) => (
    <Anchor
      size={'sm'}
      maw={'200px'}
      style={{ overflow: 'hidden', textOverflow: 'ellipsis' }}
      to={item.href}
      component={Link}
      key={index}
    >
      {t(`${item.title}`)}
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
  py = 20,
}: {
  hide?: number | null;
  py?: MantineStyleProps['py'];
  start?: { title: string; href: string };
}) => {
  const { breadCrumb } = useNav();
  const routes = useLocation();
  const pathNames = routes.pathname.split('/').splice(1);

  const newPath = pathNames.map((x, i) => {
    const elementTo = i + 1;
    if (x === 'courses') x = 'trainings';
    const href = '/' + [...pathNames].splice(0, elementTo).join('/');
    const a = {
      title: x,
      href,
    };
    return a;
  });
  const items = breadCrumb ?? newPath;
  return (
    <>
      <Breadcrumbs
        py={py}
        pb={5}
        styles={{ breadcrumb: { lineHeight: '20px' } }}
      >
        {getItems({ items: start ? [start, ...items] : items, hide })}
      </Breadcrumbs>
      <Divider />
    </>
  );
};

export default Breadcrumb;
