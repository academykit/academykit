import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Button,
  Container,
  Group,
  Loader,
  Title,
  Transition,
} from "@mantine/core";
import { useToggle } from "@mantine/hooks";
import { UserRole } from "@utils/enums";
import { useGroups } from "@utils/services/groupService";

import AddGroups from "./Components/AddGroups";
import GroupCard from "./Components/GroupCard";
import { useTranslation } from "react-i18next";

const a = {
  in: { opacity: 1 },
  out: { opacity: 0 },
  common: { transformOrigin: "top" },
  transitionProperty: "transform, opacity",
};
const b = {
  in: { opacity: 1 },
  out: { opacity: 0 },
  common: { transformOrigin: "top" },
  transitionProperty: "transform, opacity",
};

const GroupsPage = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const [showAddGroups, setShowAddGroups] = useToggle();

  const { isLoading, data } = useGroups(searchParams);
  const auth = useAuth();
  const { t } = useTranslation();
  return (
    <Container fluid>
      <Box my={10}>
        <Group
          sx={{ justifyContent: "space-between", alignItems: "center" }}
          mb={15}
        >
          <Title>{t("groups")}</Title>

          {auth?.auth && auth.auth.role <= UserRole.Admin && (
            <Transition mounted={!showAddGroups} transition={b} duration={400}>
              {(styles) => (
                <Button
                  style={{ ...styles }}
                  onClick={(e: any) => setShowAddGroups()}
                >
                  {t("add_group")}
                </Button>
              )}
            </Transition>
          )}
        </Group>

        <Transition mounted={showAddGroups} transition={a} duration={400}>
          {(styles) => (
            <>
              <Box pb={20}>
                <AddGroups onCancel={setShowAddGroups} />
              </Box>
            </>
          )}
        </Transition>

        <div style={{ display: "flex" }}>
          <Box mx={3} sx={{ width: "100%" }}>
            {searchComponent(t("search_groups") as string)}
          </Box>
        </div>
      </Box>
      <Group sx={{ justifyContent: "start" }}>
        {isLoading && <Loader />}
        {data?.data &&
          (data.data.totalCount > 0 ? (
            data?.data.items.map((x) => (
              <GroupCard search={searchParams} group={x} key={x.id} />
            ))
          ) : (
            <Box>{t("no_groups")}</Box>
          ))}
      </Group>
      {data && pagination(data.data.totalPage)}
    </Container>
  );
};

export default withSearchPagination(GroupsPage);
