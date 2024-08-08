import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Center,
  Drawer,
  Group,
  Loader,
  rem,
  ScrollArea,
  SegmentedControl,
  SimpleGrid,
  Title,
} from "@mantine/core";
import { useDisclosure } from "@mantine/hooks";
import { UserRole } from "@utils/enums";
import { useGroups } from "@utils/services/groupService";

import { IconColumns, IconLayoutGrid } from "@tabler/icons-react";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import AddGroups from "./Components/AddGroups";
import GroupCard from "./Components/GroupCard";
import GroupTable from "./Components/GroupTable";

const GroupsPage = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const [opened, { open, close }] = useDisclosure(false);

  const { isLoading, data } = useGroups(searchParams);
  const auth = useAuth();
  const { t } = useTranslation();
  const [selectedView, setSelectedView] = useState("list");

  return (
    <>
      <Box>
        <Group
          style={{ justifyContent: "space-between", alignItems: "center" }}
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
          <Group my={10}>
            <Box flex={1}>{searchComponent(t("search_groups") as string)}</Box>
            <SegmentedControl
              value={selectedView}
              onChange={setSelectedView}
              data={[
                {
                  value: "list",
                  label: (
                    <Center style={{ gap: 10 }}>
                      <IconLayoutGrid
                        style={{ width: rem(20), height: rem(20) }}
                      />
                    </Center>
                  ),
                },
                {
                  value: "table",
                  label: (
                    <Center style={{ gap: 10 }}>
                      <IconColumns
                        style={{ width: rem(20), height: rem(20) }}
                      />
                    </Center>
                  ),
                },
              ]}
            />
          </Group>
        </div>
      </Box>

      <ScrollArea>
        {data?.data &&
          (data.data.totalCount > 0 ? (
            selectedView === "table" ? (
              <GroupTable group={data.data.items} search={searchParams} />
            ) : (
              <SimpleGrid
                cols={{ sx: 1, sm: 2, md: 3, 1280: 3, 1780: 4 }}
                spacing={10}
              >
                {isLoading && <Loader />}

                {data?.data.items.map((x) => (
                  <GroupCard search={searchParams} group={x} key={x.id} />
                ))}
              </SimpleGrid>
            )
          ) : (
            <Box>{t("no_groups")}</Box>
          ))}
      </ScrollArea>
      {selectedView === "table" &&
        data &&
        pagination(data.data.totalPage, data.data.items.length)}
    </>
  );
};

export default withSearchPagination(GroupsPage);
