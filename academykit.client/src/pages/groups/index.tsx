import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Container,
  Drawer,
  Group,
  Loader,
  SimpleGrid,
  Title,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { UserRole } from "@utils/enums";
import { useGroups } from "@utils/services/groupService";

import { useTranslation } from "react-i18next";
import AddGroups from "./Components/AddGroups";
import GroupCard from "./Components/GroupCard";

const GroupsPage = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const [opened, { open, close }] = useDisclosure(false);

  const { isLoading, data } = useGroups(searchParams);
  const auth = useAuth();
  const { t } = useTranslation();
  return (
    <Container fluid>
      <Box my={10}>
        <Group
          style={{ justifyContent: "space-between", alignItems: "center" }}
          mb={15}
        >
          <Title>{t("groups")}</Title>

          {auth?.auth && Number(auth.auth.role) <= UserRole.Admin && (
            <Button onClick={open}>{t("add_group")}</Button>
          )}
        </Group>

        <Drawer
          opened={opened}
          onClose={close}
          title={t("groups")}
          overlayProps={{ backgroundOpacity: 0.5, blur: 4 }}
        >
          <AddGroups onCancel={close} />
        </Drawer>

        <div>
          <Box>{searchComponent(t("search_groups") as string)}</Box>
        </div>
      </Box>
      <SimpleGrid cols={{ sx: 1, sm: 2, md: 3, 1280: 3, 1780: 4 }} spacing={10}>
        {isLoading && <Loader />}
        {data?.data &&
          (data.data.totalCount > 0 ? (
            data?.data.items.map((x) => (
              <GroupCard search={searchParams} group={x} key={x.id} />
            ))
          ) : (
            <Box>{t("no_groups")}</Box>
          ))}
      </SimpleGrid>
      {data && pagination(data.data.totalPage, data.data.items.length)}
    </Container>
  );
};

export default withSearchPagination(GroupsPage);
