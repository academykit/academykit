import {
  Avatar,
  Box,
  Flex,
  MantineNumberSize,
  SystemProp,
  Text,
  UnstyledButton,
} from '@mantine/core';
import { PoolRole, UserRole } from '@utils/enums';
import { getInitials } from '@utils/getInitialName';
import { IUser } from '@utils/services/types';
import { CSSProperties, FC } from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

type Props = {
  user: IUser;
  size?: MantineNumberSize | undefined;
  direction?: SystemProp<CSSProperties['flexDirection']>;
  page?: string;
  color?: string;
};

const UserShortProfile: FC<Props> = ({
  user: { fullName, id, imageUrl, role },
  size = 'xl',
  direction = undefined,
  page = 'Pool',
  color = '',
}) => {
  const { t } = useTranslation();
  return (
    <UnstyledButton
      component={Link}
      to={`/userProfile/${id}/certificate`}
      sx={{ textDecoration: 'none' }}
    >
      <Flex direction={direction} gap={'md'} sx={{ alignItems: 'center' }}>
        <Avatar my={3} src={imageUrl} color="cyan" radius={10000} size={size}>
          {!imageUrl && getInitials(fullName ?? '')}
        </Avatar>
        <Box>
          <Text size={size} weight="bolder" color={color}>
            {fullName}
          </Text>
          <Text size={'sm'} color={'dimmed'}>
            {page === 'Pool'
              ? t(`${PoolRole[role] ?? ''}`)
              : t(`${UserRole[role]}`)}
          </Text>
        </Box>
      </Flex>
    </UnstyledButton>
  );
};

export default UserShortProfile;
