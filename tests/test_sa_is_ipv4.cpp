#include <gtest/gtest.h>
#include "../common.h"
#include "../sa.h"

TEST(libsatest, test_sa_is_ipv4_true) {
    sockaddr_in sa{.sin_family = AF_INET, .sin_port = htons(1000)};

    EXPECT_EQ(inet_pton(AF_INET, "1.2.3.4", &sa.sin_addr), 1);

    auto actual = sa_is_ipv4(reinterpret_cast<sockaddr *>(&sa));

    EXPECT_EQ(actual, true);
}

TEST(libsatest, test_sa_is_ipv4_false) {
    sockaddr_in6 sa{.sin6_family = AF_INET6, .sin6_port = htons(1000)};

    EXPECT_EQ(inet_pton(AF_INET6, "::ffff:0102:0304", &sa.sin6_addr), 1);

    auto actual = sa_is_ipv4(reinterpret_cast<sockaddr *>(&sa));

    EXPECT_EQ(actual, false);
}